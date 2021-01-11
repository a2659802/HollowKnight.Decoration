using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace DecorationMaster.MyBehaviour
{
    class Bench
    {
        [Serializable]
        public class BenchItem : Item
        {
            public string bench_name;
        }
            
        [Decoration("HK_bench")]
        public class Bench1 : CustomDecoration
        {
            private const string BENCH_NAME_PREFIX = "D_bench1_";
            private void Awake()
            {
                UnVisableBehaviour.AttackReact.Create(gameObject);
                
                if(string.IsNullOrEmpty(GetBenchName(item)))
                {
                    SetBenchName(item, BENCH_NAME_PREFIX + GetTimeStamp());
                }
                gameObject.name = GetBenchName(item);
                

            }
            
        }
        internal static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        internal static string GetBenchName(Item i)
        {
            return ((BenchItem)i).bench_name;
        }
        internal static void SetBenchName(Item i,string name)
        {
            ((BenchItem)i).bench_name = name;
        }
    }
}
