﻿using DecorationMaster.Attr;
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
            public override Vector3 Move(Vector3 center, Vector3 current, float speed, float span,int offset)
            {
                float dy = span * Mathf.Sin(speed * Time.time + offset * Mathf.PI);
                return new Vector3(center.x, center.y + dy, center.z);
            }
            public override void Hit(HitInstance damageInstance)
            {
                if(damageInstance.AttackType == AttackTypes.Nail)
                    base.Hit(damageInstance);
                else
                {
                    float pitch = gameObject.GetComponent<MyTinkEffect>().pitch;
                }
            }
            [Handle(Operation.SetTinkVoice)]
            public void HandleVoice(object val)
            {

            }
        }

    }
}
