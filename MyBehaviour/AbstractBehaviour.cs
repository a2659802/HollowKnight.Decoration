using DecorationMaster.Attr;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using DecorationMaster.Util;
namespace DecorationMaster.MyBehaviour
{
    public abstract class Editable : MonoBehaviour, IHitResponder
    {
        public virtual bool SetupMode { get => ItemManager.Instance.setupMode; }
        public virtual void Hit(HitInstance damageInstance)
        {
           // Logger.LogDebug($"Hit Mode:{SetupMode}");
            if (SetupMode)// && damageInstance.AttackType==AttackTypes.Nail
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
            item?.Setup(op, val);

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
            var settings = DecorationMaster.instance.ItemData;
            if (self == null)
                throw new NullReferenceException("Item Null Exception");
            item.sceneName = GameManager.instance.sceneName;
            settings.items.Add((Item)self);
        }
        [Handle(Operation.SetPos)]
        public virtual void HandlePos(Vector2 val)
        {
            gameObject.transform.position = new Vector3(val.x, val.y, gameObject.transform.position.z);
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
                try
                {
                    Setup(attr.handleType, prop.GetValue(i, null));
                }
                catch
                {
                    Logger.LogError($"An Exception occur while Setup:Op:{attr.handleType},val:{prop.GetValue(i, null)}");
                    throw new Exception("Initiate Exception");
                }
            }

            gameObject.SetActive(true);

            Logger.LogDebug($"{i.pname} Serialize");
        }
        [Handle(Operation.REMOVE)]
        public override void Remove(object self = null)
        {
            if (self == null)
                self = item;

            Logger.LogDebug($"{((Item)self).pname} - remove self");
            var settings = DecorationMaster.instance.ItemData;
            if (self == null)
                throw new NullReferenceException("Item Null Exception");

            settings.items.Remove((Item)self);
            base.Remove();
        }
        [Handle(Operation.COPY)]
        public virtual GameObject CopySelf(object self = null)
        {
            var item_clone = item.Clone() as Item;
            //var clone = Instantiate(gameObject);
            //clone.GetComponent<CustomDecoration>().item = item_clone;
            var clone = ObjectLoader.CloneDecoration(item.pname, item_clone);
            return clone;
        }
    }
    public abstract class Resizeable : CustomDecoration
    {
        [Handle(Operation.SetSize)]
        public virtual void HandleSize(float size)
        {
            gameObject.transform.localScale = size * Vector3.one;
        }
        [Handle(Operation.SetRot)]
        public virtual void HandleRot(float angle)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    public abstract class PartResizable : CustomDecoration
    {
        [Handle(Operation.SetSizeX)]
        public virtual void HandleSizeX(float size)
        {
            gameObject.transform.localScale += (size * Vector3.right);
        }

        [Handle(Operation.SetSizeY)]
        public virtual void HandleSizeY(float size)
        {
            gameObject.transform.localScale += (size * Vector3.up);
        }
        [Handle(Operation.SetRot)]
        public virtual void HandleRot(float angle)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    public abstract class SawMovement : Resizeable
    {
        private void Update()
        {
            var sitem = item as ItemDef.SawItem;
            var nextPoint = Move(gameObject.transform.position);
            gameObject.transform.position = nextPoint;
        }
        public abstract Vector3 Move(Vector3 current);
        public override void HandleRot(float angle)
        {
            return;
        }
    }

    public abstract class BoolBinding : Resizeable
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
        private void Awake()
        {
            gameObject.AddComponent<NonBouncer>();
        }
        public override void Hit(HitInstance hit)
        {
            base.Hit(hit);
            Destroy(gameObject);
        }
    }

    public abstract class IntBinding : Resizeable
    {
        public virtual string BindIntValue { get; private set; }
        private void OnEnable()
        {
            if (BindIntValue != null)
                ModHooks.Instance.GetPlayerIntHook += Bind;
        }
        private void OnDisable()
        {
            if (BindIntValue != null)
                ModHooks.Instance.GetPlayerIntHook -= Bind;
        }
        public int Bind(string name)
        {
            return name == BindIntValue ? 0 : PlayerData.instance.GetIntInternal(name);
        }
    }
    public abstract class BreakableIntBinding : IntBinding
    {
        private void Awake()
        {
            gameObject.AddComponent<NonBouncer>();
        }
        public override void Hit(HitInstance hit)
        {
            base.Hit(hit);
            Destroy(gameObject);
        }
    }
}
