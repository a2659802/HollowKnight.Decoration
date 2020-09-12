using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
namespace DecorationMaster
{
    [Serializable]
    public abstract class Item : ICloneable
    {
        [InspectIgnore]
        public string sceneName;
        public virtual string pname { get; set; }
        [InspectIgnore]
        public V2 position;
        //public float x;
        //public float y;
        public virtual void Setup(Operation op, object val)
        {
            /*
            switch (op)
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

            //var handlers = this.GetType().GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().Where(x => x.handleType == op);
            // Find a method with HandleAttribue which can handle current operation
            var handlers = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x=>x.GetCustomAttributes(typeof(HandleAttribute),true).OfType<HandleAttribute>()
                .Where(y=>y.handleType == op).Any());
            //Logger.LogDebug($"Get Method:{handlers.FirstOrDefault()} in type:{GetType()} while handle Operation {op.ToString()}");
            foreach(var m in handlers)
            {
                object[] args = new object[] { val, };
                m.Invoke(this, args);
                break; // if there more than one method can handle an operation, this inst should be removed
            }
        }

        [Handle(Operation.Serialize)]
        public virtual void HandleInit(Item dat)
        {
            pname = dat.pname;
            HandlePos(dat.position);
        }
        
        [Handle(Operation.SetPos)]
        public virtual void HandlePos(Vector2 p)
        {
            position = new V2(p);
        }

        public object Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
    [Serializable]
    public abstract class ResizableItem : Item
    {
        public float size = 1;
        public float angle;

        /*public override void Setup(Operation op, object val)
        {
            base.Setup(op, val);
            switch(op)
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
            this.size = size;
        }
        [Handle(Operation.SetRot)]
        public void HandleRot(float angle)
        {
            this.angle = angle;
        }
    }
    
    public class ItemDef
    {
        [Serializable]
        [Decoration("default")]
        public class DefaultItem : Item
        {
        }
        [Serializable]
        [Decoration("HK_saw")]
        public class SawItem : ResizableItem
        {
            public int span = 3;
            public int speed;
            [InspectIgnore]
            public V2 Center;
            /*public override void Setup(Operation op, object val)
            {
                base.Setup(op, val);
                switch(op)
                {
                    case Operation.SetSpan:
                        HandleSpan((int)val);
                        break;
                    case Operation.SetSpeed:
                        HandleSpeed((int)val);
                        break;
                    default:
                        break;
                }
            }*/
            [Handle(Operation.SetSpan)]
            public void HandleSpan(int span)
            {
                this.span = span;
            }
            [Handle(Operation.SetSpeed)]
            public void HandleSpeed(int speed)
            {
                this.speed = speed;
            }
            public override void HandleInit(Item dat)
            {
                base.HandleInit(dat);
                var sitem = dat as SawItem;
                HandleSpan(sitem.span);
                HandleSpeed(sitem.speed);
            }
            [Handle(Operation.SetPos)]
            public override void HandlePos(Vector2 p)
            {
                base.HandlePos(p);
                Center = new V2(p);
            }
        }
    }
}
