﻿using DecorationMaster.Attr;
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
using DecorationMaster.UI;
namespace DecorationMaster
{
    [Serializable]
    public abstract class Item : ICloneable
    {
        [InspectIgnore]
        public string sceneName { get; set; }
        public virtual string pname { get; set; }

        [InspectIgnore]
        [Handle(Operation.SetPos)]
        public V2 position { get; set; }
        public void Setup(Operation op, object val)
        {
            var handle_prop = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>()
                .Where(y => y.handleType == op).Any());

            foreach (var prop in handle_prop)
            {
                try
                {
                    if(val.GetType()==typeof(Vector2))
                    {
                        prop.SetValue(this, new V2((Vector2)val),null);
                    }
                    else
                    {
                        prop.SetValue(this, val, null);
                    }
                }
                catch (ArgumentException e)
                {
                    Logger.LogDebug($" ### Val Type:{val.GetType()},Prop Type:{prop.PropertyType}");
                    throw e;
                }
                
                /*
                var setter = prop.GetSetMethod();
                setter.Invoke(this, new object[] { val, });*/
            }
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
        [Handle(Operation.SetSize)]
        [FloatConstraint(0.2f,2f)]
        public float size { get; set; } = 1;

        [Handle(Operation.SetRot)]
        [IntConstraint(0,360)]
        public int angle { get; set; } = 0;

    }
    
    public class ItemDef
    {
        [Serializable]
        [Decoration("default")]
        public class DefaultItem : ResizableItem
        {
        }

        [Serializable]
        [Decoration("HK_saw")]
        public class SawItem : ResizableItem
        {
            [Handle(Operation.SetSpan)]
            [IntConstraint(0,5)]
            public int span { get; set; } = 3;

            [Handle(Operation.SetSpeed)]
            [IntConstraint(-3,3)]
            public int speed { get; set; }

            [Handle(Operation.SetTinkVoice)]
            [FloatConstraint(0.1f,1)]
            public float pitch { get; set; } = 1;

            [Handle(Operation.SetPos)]
            [InspectIgnore]
            public V2 Center { get; set; }

            [Handle(Operation.SetOffset)]
            [IntConstraint(0,5)]
            public int offset { get; set; }
        }
    }
}
