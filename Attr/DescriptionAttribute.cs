using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Attr
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property,AllowMultiple =true,Inherited =true)]
    public class DescriptionAttribute : Attribute
    {
        public string Language;
        public string Text;
        public DescriptionAttribute(string text, string language = "zh-cn")
        {
            Text = text;
            Language = language;
        }
        public bool IsChinese()
        {
            return Language == "zh-cn";
        }
    }

}
