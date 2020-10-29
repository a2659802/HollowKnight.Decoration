using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DecorationMaster.Attr;

namespace DecorationMaster.Util
{
    public static class ReflectionCache
    {
        public readonly static Dictionary<Type, Dictionary<Operation, List<PropertyInfo>>> ItemPropCache = new Dictionary<Type, Dictionary<Operation, List<PropertyInfo>>>();
        public readonly static Dictionary<Type, Dictionary<Operation, List<MethodInfo>>> BehaviourMethodCache = new Dictionary<Type, Dictionary<Operation, List<MethodInfo>>>();

        public static List<PropertyInfo> GetItemProps(Type t,Operation op)
        {
            
            if(ItemPropCache.TryGetValue(t,out var props))
            {
                if (props.TryGetValue(op, out var res))
                    return res;
                else
                    return null; 
            }
            var handle_prop = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).Any());
            Dictionary<Operation, List<PropertyInfo>> opbind = new Dictionary<Operation, List<PropertyInfo>>();
            foreach(var p in handle_prop)
            {
                Operation p_op = p.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().FirstOrDefault().handleType;
                if (opbind.TryGetValue(p_op, out var list))
                {
                    list.Add(p);
                }
                else
                {
                    opbind.Add(p_op, new List<PropertyInfo> { p });
                }
                //Logger.LogDebug($"Cache: {p_op}:{p.Name},{t}");
            }
            ItemPropCache.Add(t, opbind);

            if (ItemPropCache.TryGetValue(t, out var props2))
            {
                if (props2.TryGetValue(op, out var res2))
                    return res2;
                else
                    return null;
            }
            else
                return null;
        }

        public static List<MethodInfo> GetMethods(Type t, Operation op)
        {

            if (BehaviourMethodCache.TryGetValue(t, out var props))
            {
                if (props.TryGetValue(op, out var res))
                    return res;
                else
                    return null;
            }
            var handle_methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttributes(typeof(HandleAttribute), true).Any());
            Dictionary<Operation, List<MethodInfo>> opbind = new Dictionary<Operation, List<MethodInfo>>();
            foreach (var m in handle_methods)
            {
                var ops = m.GetCustomAttributes(typeof(HandleAttribute), true).OfType<HandleAttribute>().Select(x=>x.handleType);//.FirstOrDefault().handleType;
                foreach(var p_op in ops)
                {
                    if (opbind.TryGetValue(p_op, out var list))
                    {
                        list.Add(m);
                    }
                    else
                    {
                        opbind.Add(p_op, new List<MethodInfo> { m });
                    }
                    
                }

            }
            BehaviourMethodCache.Add(t, opbind);

            if (BehaviourMethodCache.TryGetValue(t, out var props2))
            {
                if (props2.TryGetValue(op, out var res2))
                    return res2;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
