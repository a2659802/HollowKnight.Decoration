using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;
using DecorationMaster;
namespace DecorationMaster.MyBehaviour
{
    public class MovablePlatform
    {
        [Decoration("move_flip_platform")]
        public class MoveFilp : SawMovement
        {
            private void Awake()
            {
                if(ObjectLoader.InstantiableObjects.TryGetValue("HK_flip_platform",out var flip))
                {
                    var fgo = Instantiate(flip);
                    fgo.transform.SetParent(gameObject.transform);
                    fgo.SetActive(true);
                }
            }
            public override Vector3 Move(Vector3 current)
            {
                var sitem = item as ItemDef.SawItem;

                float dealtDis = sitem.span * Mathf.Sin(sitem.speed * Time.time + sitem.offset * Mathf.PI / 2);
                float dx = dealtDis * Mathf.Cos(sitem.angle * Mathf.PI / 180f);
                float dy = dealtDis * Mathf.Sin(sitem.angle * Mathf.PI / 180f);
                return new Vector3(sitem.Center.x + dx, sitem.Center.y + dy, current.z);
            }
        }
    }
}
