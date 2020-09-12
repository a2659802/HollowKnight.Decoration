using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DecorationMaster
{
    // TO DO Inspector UI
    class Inspector
    {
        private static Dictionary<Type,string> _inspectFields(object obj,BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            Dictionary<Type, string> fi = new Dictionary<Type, string>();
            FieldInfo [] fields = obj.GetType().GetFields(flags);
            foreach(var f in fields)
            {
                Type t = f.GetType();
                string name = f.Name;
                fi.Add(t, name);
            }
            return fi;
        }
        private static void InspectFields(Item item)
        {
            var fi = _inspectFields(item);

        }
    }
}
