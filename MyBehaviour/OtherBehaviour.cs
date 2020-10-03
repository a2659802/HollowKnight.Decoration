using DecorationMaster.Attr;
using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Util;
using TMPro;

namespace DecorationMaster.MyBehaviour
{
    public class OtherBehaviour
    {
        [Decoration("HK_saw")]
        public class Saw : SawMovement
        {
            public override Vector3 Move(Vector3 current)
            {
                var sitem = item as ItemDef.SawItem;

                float dealtDis = sitem.span * Mathf.Sin(sitem.speed * Time.time + sitem.offset * Mathf.PI/2);
                float dx = dealtDis * Mathf.Cos(sitem.angle * Mathf.PI / 180f);
                float dy = dealtDis * Mathf.Sin(sitem.angle * Mathf.PI / 180f);
                return new Vector3(sitem.Center.x + dx , sitem.Center.y + dy, current.z);
            }
            public override void Hit(HitInstance damageInstance)
            {
                if(damageInstance.AttackType == AttackTypes.Nail)
                    base.Hit(damageInstance);
                else
                {
                    //float pitch = gameObject.GetComponent<MyTinkEffect>().pitch;
                }
            }
            //[Handle(Operation.SetTinkVoice)]
            public void HandleVoice(object val)
            {

            }
        }

        [Decoration("HK_fly")]
        public class Fly: Resizeable
        {
            public void OnTriggerEnter2D(Collider2D col)
            {
                if(col.name.Contains("Slash"))
                {
                    if (SetupMode)
                        Remove();
                }
            }

        }
        [Decoration("HK_turret")]
        public class Turret : Resizeable
        {
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.name.Contains("Slash"))
                {
                    if (SetupMode)
                        Remove();
                }
            }

        }
        [Decoration("HK_lever")]
        public class Lever : Resizeable
        {
            private GameObject numDisp;
            private PlayMakerFSM playMakerFSM;
            public void Awake()
            {
                playMakerFSM = gameObject.LocateMyFSM("toll switch");
                playMakerFSM.SetState("Pause");
            }
            public void Start()
            {
                gameObject.transform.eulerAngles = new Vector3(0, 0, ((ResizableItem)item).angle);
                int gateNum = ((ItemDef.LeverItem)item).GateNumber;
                var gateName = $"{ItemDef.LeverItem.GateNamePrefix}{gateNum}";
                playMakerFSM.GetAction<FindGameObject>("Initiate", 2).objectName = gateName;

                if(ItemManager.Instance.setupMode)
                    numDisp = NameDisp.Create(gameObject, $"{gateNum}");
            }
            [Handle(Operation.SetGate)]
            public void HandleGateNumber(int num)
            {
                if (numDisp != null)
                {
                    numDisp.GetComponent<TextMeshPro>().text = num.ToString();
                }
            }
        }
        [Decoration("HK_gate")]
        public class Gate : Resizeable
        {
            private GameObject numDisp;
            public void Start()
            {
                int gateNum = ((ItemDef.GateItem)item).GateNumber;
                var gateName = $"{ItemDef.GateItem.GateNamePrefix}{gateNum}";
                gameObject.name = gateName;

                if(ItemManager.Instance.setupMode)
                    numDisp = NameDisp.Create(gameObject, $"{gateNum}");
            }

            [Handle(Operation.SetGate)]
            public void HandleGateNumber(int num)
            {
                if(numDisp!=null)
                {
                    numDisp.GetComponent<TextMeshPro>().text = num.ToString();
                }
            }
        }

    }
}
