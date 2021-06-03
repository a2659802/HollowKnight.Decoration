using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property,AllowMultiple =false,Inherited =true)]
    public abstract class ConstraintAttribute : Attribute
    {
        public virtual object Min { get; private set; }
        public virtual object Max { get; private set; }

    }
    public sealed class FloatConstraint : ConstraintAttribute
    {
        private float min;
        private float max;
        public override object Min => min;
        public override object Max => max;

        public FloatConstraint(float min,float max)
        {
            this.min = min;
            this.max = max;
        }
    }
    public sealed class IntConstraint : ConstraintAttribute
    {
        private int min;
        private int max;
        public override object Min => min;
        public override object Max => max;
        public IntConstraint(int min,int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    //min max consider to string length
    public sealed class StringConstraint : ConstraintAttribute
    {
        private uint min;
        private uint max;
        public override object Min => min;
        public override object Max => max;
        public StringConstraint(uint min,uint max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
