using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = false)]
    public sealed class DecorationAttribute : Attribute
    {
        public string Name { get; }
        public DecorationAttribute(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
        /*public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!obj.GetType().Equals(typeof(DecorationAttribute)))
                return false;
            DecorationAttribute d = obj as DecorationAttribute;
            return Name.Equals(d.Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }*/
    }
}
