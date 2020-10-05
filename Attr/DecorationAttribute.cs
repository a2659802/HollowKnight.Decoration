using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
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
        public override bool Equals(object obj)
        {
            return (obj as DecorationAttribute)?.Name == this.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public static bool operator ==(DecorationAttribute a, DecorationAttribute b)
        {
            if (a is null)
                return b is null;
            return a.Equals(b);
        }
        public static bool operator !=(DecorationAttribute a, DecorationAttribute b)
        {
            return !(a==b);
        }
    }
}
