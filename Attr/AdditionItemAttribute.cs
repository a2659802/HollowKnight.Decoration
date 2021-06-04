using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class AdditionItemAttribute:Attribute
    {
        public Type type;
        public AdditionItemAttribute(Type item_type)
        {
            type = item_type;
        }
    }
}
