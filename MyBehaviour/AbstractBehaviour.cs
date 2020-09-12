using DecorationMaster.Attr;
using Modding;
using System;
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
            if (SetupMode)
                Remove();
        }
        public abstract void Add(); //add this item to global
        public virtual void Remove() //remove this item from global
        {
            Destroy(gameObject);
        }
    }

    public abstract class CustomDecoration : Editable
    {
        public Item item;
        /// <summary>
        /// Setup the value of item, and do some effect base on item
        /// </summary>
        /// <param name="op"></param>
        /// <param name="val"></param>
        public virtual void Setup(Operation op, object val)
        {
            item.Setup(op, val);

            /*switch (op)
            {
                case Operation.SetPos:
                    HandlePos((Vector2)val);
                    break;
                case Operation.Serialize:
                    HandleInit((Item)val);
                    break;
                default:
                    break;
            }*/
            var handlers = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>()
                .Where(y => y.handleType == op).Any());
            //Logger.LogDebug($"Get Method:{handlers.FirstOrDefault()} in type:{GetType()} while handle Operation {op.ToString()}");
            foreach (var m in handlers)
            {
                object[] args = new object[] { val, };
                m.Invoke(this, args);
                break; // if there more than one method can handle an operation, this inst should be removed
            }
        }
        /// <summary>
        /// Add Object's Setting To Global Settings
        /// </summary>
        public override void Add()
        {
            var settings = DecorationMaster.instance.Settings;
            if (item == null)
                throw new NullReferenceException("Item Null Exception");
            item.sceneName = GameManager.instance.sceneName;
            settings.items.Add(item);
        }
        [Handle(Operation.SetPos)]
        public virtual void HandlePos(Vector2 val)
        {
            gameObject.transform.position = val;
        }
        [Handle(Operation.Serialize)]
        public virtual void HandleInit(Item i)
        {
            if (item != i)
                item = i;

            HandlePos(i.position);
            gameObject.SetActive(true);
        }

        public override void Remove()
        {
            Logger.LogDebug("remove self");
            var settings = DecorationMaster.instance.Settings;
            if (item == null)
                throw new NullReferenceException("Item Null Exception");
            settings.items.Remove(item);
            base.Remove();
        }
        
    }
    public abstract class Resizeable : CustomDecoration
    {
        /*public override void Setup(Operation op, object val)
        {
            base.Setup(op, val);
            switch (op)
            {
                case Operation.SetSize:
                    HandleSize((float)val);
                    break;
                case Operation.SetRot:
                    HandleRot((float)val);
                    break;
                default:
                    break;
            }
        }*/
        public override void HandleInit(Item dat)
        {
            base.HandleInit(dat);
            ResizableItem ritem = dat as ResizableItem;
            HandleSize(ritem.size);
            HandleRot(ritem.angle);
        }
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
            var nextPoint = Move(sitem.Center, gameObject.transform.position, sitem.speed,sitem.span);
            gameObject.transform.position = nextPoint;
            
        }
        public abstract Vector3 Move(Vector3 center, Vector3 current, float speed, float span);

        public override void HandlePos(Vector2 val)
        {
            //set center pos instead of real pos
            var sitem = item as ItemDef.SawItem;
            sitem.Center = val;
        }
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
