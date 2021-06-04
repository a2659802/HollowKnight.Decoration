using DecorationMaster.Attr;
using DecorationMaster.Util;
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
                    SetBenchName(item, BENCH_NAME_PREFIX + IdGenerator.Unique);
                }
                gameObject.name = GetBenchName(item);
            }
            public override GameObject CopySelf(object self = null)
            {
                var clone = base.CopySelf(self);
                clone.name += "(copy)" + IdGenerator.Unique;
                return clone;
            }

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
