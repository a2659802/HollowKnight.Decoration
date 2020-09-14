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
        public Type ArgType;
        /// <summary>
        /// use this to handle Setup(Operation op,object val)
        /// </summary>
        /// <param name="op"> Operation Enum</param>
        /// <param name="argType">Operation calling argument's type</param>
        public HandleAttribute(Operation op,Type argType = default)
        {
            handleType = op;
            ArgType = argType;
        }
    }
}
