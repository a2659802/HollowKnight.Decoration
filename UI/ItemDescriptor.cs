using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecorationMaster.MyBehaviour;
using DecorationMaster.Attr;
namespace DecorationMaster.UI
{
    // never use 
    public static class ItemDescriptor
    {
        public static readonly Dictionary<string, string> description = new Dictionary<string, string>();

        public static void Register(Type t,string poolname)
        {
            /*if(t.IsSubclassOf(typeof(CustomDecoration)))
            {
                var desc = t.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>();
                foreach (var d in desc)
                {
                    var lang = d.Language;
                    var text = d.Text;
                    string key = null;

                    if (lang == "zh-cn")
                        key = $"{poolname}_zh-cn";
                    else if (lang == "en-us")
                        key = $"{poolname}_en-us";

                    if (description.TryGetValue(key, out _))
                        description[key] = text;
                    else
                        description.Add(key, text);
                }
                Logger.LogDebug($"Add Desc to {t}:{GetDescription(poolname,t)}");
            }*/
        }
        public static string GetDescription(string poolname,Type t=null, string language = "zh-cn")
        {
            if(description.TryGetValue($"{poolname}_{language}",out string desc))
            {
                return desc;
            }
            return null;
        }
        public static void Register<T>()
        {

        }
    }
}
