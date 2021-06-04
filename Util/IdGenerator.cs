using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecorationMaster.Util
{
    public static class IdGenerator
    {
        public static string Unique 
        { 
            get 
            {
                return $"{Range()}_{TimeStamp.Substring(2)}";
            } 
        }

        public static string TimeStamp
        {
            get
            {
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }
        }
        
        public static string Range(int min = 1,int max = 999999)
        {
            return UnityEngine.Random.Range(min, max).ToString();
        }

        public static string Group(string group_name)
        {
            return $"{group_name}_{Unique}";
        }
    }
}
