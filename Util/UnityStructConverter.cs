using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DecorationMaster.Util
{
    public class UnityStructConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            //return true;
            return objectType.IsValueType && objectType.FullName.Contains("UnityEngine.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);
            var res = Activator.CreateInstance(objectType);
            foreach (var f in jobj)
            {
                var field = objectType.GetField(f.Key, BindingFlags.Public | BindingFlags.Instance);
                field.SetValue(res, Convert.ChangeType(((JValue)f.Value).Value, field.FieldType));
            }
            return res;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jobj = new JObject();
            //jobj.Add("$type", value.GetType().FullName);
            var fields = value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var f in fields)
            {
                Type ftype = f.FieldType;
                if (ftype == typeof(float))
                {
                    jobj.Add(f.Name, (float)f.GetValue(value));
                }
                else if (ftype == typeof(double))
                {
                    jobj.Add(f.Name, (double)f.GetValue(value));
                }
                else if (ftype == typeof(int))
                {
                    jobj.Add(f.Name, (int)f.GetValue(value));
                }
                else if (ftype == typeof(long))
                {
                    jobj.Add(f.Name, (long)f.GetValue(value));
                }
            }

            jobj.WriteTo(writer);
        }
    }
}
