using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited = false)]
    public sealed class InspectIgnoreAttribute : Attribute
    {
    }
}
