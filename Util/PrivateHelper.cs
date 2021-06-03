using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace DecorationMaster.Util
{
    public static class PrivateHelper
    {
        private static Dictionary<(Type, string), MethodInfo> method_cache;
        private static Dictionary<(Type, string), FieldInfo> field_cache;
        public static object Call(object obj, string methodName, params object[] args)
        {
            if (method_cache == null)
            {
                method_cache = new Dictionary<(Type, string), MethodInfo>();
            }
            var type = obj.GetType();
            if(!method_cache.TryGetValue((type,methodName),out MethodInfo m))
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => (x.Name == methodName && x.GetParameters().Length == args.Length)).ToArray();

                if(methods.Length == 1)
                {
                    m = methods[0];
                }
                else
                {
                    for(int i=0;i<methods.Length;i++)
                    {
                        var para = methods[i].GetParameters();
                        bool match = true;
                        for(int j=0;j<para.Length;j++)
                        {
                            var method_para_type = para[j].ParameterType;
                            var arg_type = args[j].GetType();
                            match &= (method_para_type == arg_type);
                            if (!match)
                                break;
                        }
                        if(match)
                        {
                            m = methods[i];
                            break;
                        }
                    }
                }
                method_cache.Add((type, methodName), m);
            }
            return m.Invoke(obj, args);

            //var m = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        }

        public static void SetField(object obj,string fieldName,object value)
        {
            if (field_cache == null)
            {
                field_cache = new Dictionary<(Type, string), FieldInfo>();
            }
            var type = obj.GetType();
            if(!field_cache.TryGetValue((type,fieldName),out FieldInfo f))
            {
                f = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                field_cache.Add((type, fieldName), f);
            }
            //var f = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            f.SetValue(obj, value);
        }
    }
}
