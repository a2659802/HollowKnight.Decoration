﻿using DecorationMaster.Attr;
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

                float dealtDis = sitem.span * Mathf.Sin(sitem.speed * Time.time + sitem.offset * Mathf.PI / 2);
                float dx = dealtDis * Mathf.Cos(sitem.angle * Mathf.PI / 180f);
                float dy = dealtDis * Mathf.Sin(sitem.angle * Mathf.PI / 180f);
                return new Vector3(sitem.Center.x + dx, sitem.Center.y + dy, current.z);
            }
            public override void Hit(HitInstance damageInstance)
            {
                if (damageInstance.AttackType == AttackTypes.Nail)
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
            public override void HandlePos(Vector2 val)
            {
                return;
            }
        }

        [Decoration("HK_fly")]
        public class Fly : Resizeable
        {
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK)
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
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK)
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
                int gateNum = ((ItemDef.LeverGateItem)item).Number;
                var gateName = $"{ItemDef.LeverGateItem.GateNamePrefix}{gateNum}";
                playMakerFSM.GetAction<FindGameObject>("Initiate", 2).objectName = gateName;

                if (ItemManager.Instance.setupMode)
                {
                    numDisp = NameDisp.Create(gameObject, $"{gateNum}");

                    if (gateNum == 0)
                    {
                        var exists = FindObjectsOfType<Lever>().Length;
                        Setup(Operation.SetGate, exists);
                    }
                }
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
                int gateNum = ((ItemDef.LeverGateItem)item).Number;
                var gateName = $"{ItemDef.LeverGateItem.GateNamePrefix}{gateNum}";
                gameObject.name = gateName;

                if (ItemManager.Instance.setupMode)
                {
                    numDisp = NameDisp.Create(gameObject, $"{gateNum}");

                    if (gateNum == 0)
                    {
                        var exists = FindObjectsOfType<Gate>().Length;
                        Setup(Operation.SetGate, exists);
                    }
                }
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

        [Decoration("IMG_RespawnPoint")]
        public class RespawnTrigger : Resizeable
        {
            private HazardRespawnMarker respawnMarker;
            private void Awake()
            {
                gameObject.transform.localScale *= 2.5f;
                gameObject.layer = (int)GlobalEnums.PhysLayers.PROJECTILES;
                respawnMarker = gameObject.AddComponent<HazardRespawnMarker>();
            }
            private void Start()
            {
                if (!SetupMode)
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            private void OnTriggerEnter2D(Collider2D otherCollider)
            {
                int layer = otherCollider.gameObject.layer;
                if (layer == (int)GlobalEnums.PhysLayers.PLAYER || layer == (int)GlobalEnums.PhysLayers.HERO_BOX)
                {
                    PlayerData.instance.SetHazardRespawn(respawnMarker);
                }
            }

        }

        [Decoration("HK_break_wall")]
        public class BreakWall : Resizeable
        {
            private void Awake()
            {
                var fsm = gameObject.GetComponent<PlayMakerFSM>();
                fsm.RemoveTransition("Initiate", "ACTIVATE");
                fsm.RemoveAction("Initiate", 11);
                fsm.SetState("Pause");
            }
        }

        [Decoration("HK_unbreak_wall")]
        public class UnBreakWall : Resizeable
        {
            private void Awake()
            {
                var bw = Instantiate(ObjectLoader.InstantiableObjects["HK_break_wall"]);
                var fsm = bw.GetComponent<PlayMakerFSM>();
                fsm.RemoveTransition("Initiate", "ACTIVATE");
                fsm.RemoveAction("Initiate", 11);
                fsm.SetState("Pause");
                bw.transform.SetParent(gameObject.transform);
                bw.transform.localPosition = Vector3.zero;
                Destroy(bw.GetComponent<PlayMakerFSM>());
                bw.SetActive(true);
                //Logger.LogDebug(bw.transform.position);
                //bw.AddComponent<ShowColliders>();
            }
        }
        //[Decoration("HK_inspect_region")]
        public class InspectRegion : UnVisableBehaviour
        {
            public void Start()
            {
                gameObject.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmString("Game Text Convo").Value = "Decoration_Test";
            }
        }
    }
}
