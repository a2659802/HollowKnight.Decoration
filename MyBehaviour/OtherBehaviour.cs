using DecorationMaster.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DecorationMaster.MyBehaviour
{
    public class OtherBehaviour
    {
        [Decoration("HK_saw")]
        public class Saw : SawMovement
        {
            public override Vector3 Move(Vector3 center, Vector3 current, float speed, float span)
            {
                float dy = span * Mathf.Sin(Time.time * speed);
                return new Vector3(center.x, center.y + dy, center.z);
            }

        }
    }
}
