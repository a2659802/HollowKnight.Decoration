using DecorationMaster.Attr;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DecorationMaster
{
    public abstract class Editable : MonoBehaviour, IHitResponder
    {
        public virtual bool SetupMode { get => ItemManager.Instance.setupMode; }
        public virtual void Hit(HitInstance damageInstance)
        {
           // Logger.LogDebug($"Hit Mode:{SetupMode}");
            if (SetupMode)
                Remove();
        }
        public abstract void Add(object self=null); //add this item to global
        public virtual void Remove(object self=null) //remove this item from global
        {
            
            if (self == null)
                Destroy(gameObject);
            else
            {
                try
                {
                    Destroy(self as GameObject);
                }
                catch
                {
                    Logger.LogWarn("An Exception ocurr on Editable.Remove()");
                }
            }
        }
    }

    public abstract class CustomDecoration : Editable
    {
        public Item item;
        /// <summary>
        /// Setup the value of item, and do some effect base on item
        /// To Deal with An Operation, you must Add a Method with HandleAttribute that match OP enum
        /// </summary>
        /// <param name="op"></param>
        /// <param name="val">the type must base on Item Prop</param>
        public object Setup(Operation op, object val)
        {
            item.Setup(op, val);

            var handlers = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>()
                .Where(y => y.handleType == op).Any());

            object _return = null;
            foreach (var m in handlers)
            {
                object[] args;
                if (val == null)
                {
                    ArrayList objList = new ArrayList();
                    for (int i = 0; i < m.GetParameters().Length; i++)
                        objList.Add(null);
                    args = objList.ToArray();
                    Logger.LogDebug($"argument null,fill with {args.Length} null");
                } 
                else
                    args = new object[] { val.GetType() == typeof(V2) ? ((Vector2)((V2)val)) : val, };
                object mechod_ret = m.Invoke(this, args);
                _return = mechod_ret == null ? _return: mechod_ret;
            }
            return _return;
        }
        /// <summary>
        /// Add Object's Setting To Global Settings
        /// </summary>
        [Handle(Operation.ADD)]
        public override void Add(object self = null)
        {
            if (self == null)
                self = item;
            var settings = DecorationMaster.instance.Settings;
            if (self == null)
                throw new NullReferenceException("Item Null Exception");
            item.sceneName = GameManager.instance.sceneName;
            settings.items.Add((Item)self);
        }
        [Handle(Operation.SetPos)]
        public virtual void HandlePos(Vector2 val)
        {
            gameObject.transform.position = val;
        }
        [Handle(Operation.Serialize)]
        public void HandleInit(Item i)
        {
            if (item != i)
                item = i;

            //search all Handleable Property in item
            var handlableProps = i.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().Any());

            foreach(var prop in handlableProps)
            {
                HandleAttribute attr = prop.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().FirstOrDefault();
                if (attr == null || attr.handleType==Operation.None)
                    continue;
                Setup(attr.handleType,prop.GetValue(i, null));
            }

            gameObject.SetActive(true);

            Logger.LogDebug($"{i.pname} Serialize");
        }
        [Handle(Operation.REMOVE)]
        public override void Remove(object self = null)
        {
            Logger.LogDebug($"{((Item)self).pname} - remove self");

            if (self == null)
                self = item;
            
            var settings = DecorationMaster.instance.Settings;
            if (self == null)
                throw new NullReferenceException("Item Null Exception");

            settings.items.Remove((Item)self);
            base.Remove();
        }
        [Handle(Operation.COPY)]
        public GameObject CopySelf(object self = null)
        {
            var item_clone = item.Clone() as Item;
            var clone = Instantiate(gameObject);
            clone.GetComponent<CustomDecoration>().item = item_clone;
            return clone;
        }
    }
    public abstract class Resizeable : CustomDecoration
    {
        [Handle(Operation.SetSize)]
        public void HandleSize(float size)
        {
            gameObject.transform.localScale *= size;
        }
        [Handle(Operation.SetRot)]
        public void HandleRot(float angle)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    public abstract class SawMovement : CustomDecoration
    {
        private void Start()
        {
            
        }
        private void Update()
        {
            var sitem = item as ItemDef.SawItem;
            var nextPoint = Move(sitem.Center, gameObject.transform.position, sitem.speed,sitem.span,sitem.offset);
            gameObject.transform.position = nextPoint;
            
        }
        public abstract Vector3 Move(Vector3 center, Vector3 current, float speed, float span,int offset);
    }

    public abstract class BoolBinding : CustomDecoration
    {
        public virtual string BindBoolValue { get; private set; }
        private void OnEnable()
        {
            if (BindBoolValue != null)
                ModHooks.Instance.GetPlayerBoolHook += Bind;
        }
        private void OnDisable()
        {
            if (BindBoolValue != null)
                ModHooks.Instance.GetPlayerBoolHook -= Bind;
        }
        public bool Bind(string name)
        {
            return name == BindBoolValue ? false : PlayerData.instance.GetBoolInternal(name);
        }
    }
    
    public abstract class BreakableBoolBinding : BoolBinding
    {
        public override void Hit(HitInstance hit)
        {
            base.Hit(hit);
            Destroy(gameObject);
        }
    }
}
