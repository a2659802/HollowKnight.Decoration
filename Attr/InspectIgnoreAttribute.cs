using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple =false,Inherited = false)]
    public sealed class InspectIgnoreAttribute : Attribute
    {
    }
}
