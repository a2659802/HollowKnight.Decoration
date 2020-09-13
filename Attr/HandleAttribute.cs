using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property ,AllowMultiple =true,Inherited =true)]
    public class HandleAttribute  : Attribute
    {
        public Operation handleType;
        public HandleAttribute(Operation op)
        {
            handleType = op;
        }
    }
}
