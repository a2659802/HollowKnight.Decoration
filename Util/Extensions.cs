using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DecorationMaster.Util
{
    public static class Extensions
    {
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
           => (x, y, z) = (v.x, v.y, v.z);

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> self, out TKey key, out TValue value)
            => (key, value) = (self.Key, self.Value);

    }

}
