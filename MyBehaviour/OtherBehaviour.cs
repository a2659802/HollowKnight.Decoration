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
            //[Handle(Operation.SetTinkVoice)]
            public void HandleVoice(object val)
            {

            }
        }

        //[Decoration("HK_fly")]
        public class Fly:CustomDecoration
        {
            void OnTriggerEnter2D(Collider2D col)
            {
                Logger.LogDebug($"Trigger {col.name}");
            }
            void OnCollisionEnter2D(Collision2D col)
            {
                Logger.LogDebug($"Collistion {col.collider.name}");
            }
            public void Awake()
            {
                Logger.LogDebug("Awake FlyFlyFlyFlyFlyFlyFlyFlyFlyFly");
            }
            public void OnDestroy()
            {
                Logger.LogDebug("DIE FlyFlyFlyFlyFlyFlyFlyFlyFlyFly");
            }
        }
    }
}
