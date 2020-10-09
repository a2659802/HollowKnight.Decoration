using DecorationMaster.Attr;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using DecorationMaster.Util;
using TMPro;

namespace DecorationMaster.MyBehaviour
{
    public class OtherBehaviour
    {
        [Description("电锯，是这个MOD最初的装饰品，也是MOD名字——装修大师 的由来")]
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

        [Description("放置时可能会有些bug:到处飘，这个只需要重新进入场景就不会出现了")]
        [Decoration("HK_fly")]
        public class Fly : Resizeable
        {
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK && col.name.Contains("Slash"))
                {
                    if (SetupMode)
                        Remove();
                }
            }

        }

        [Description("由于放置时它把自己炸死会出BUG，所以我把血量设为了9999")]
        [Decoration("HK_turret")]
        public class Turret : Resizeable
        {
            private void Awake()
            {
                var hm = gameObject.GetComponent<HealthManager>();
                if (hm)
                    hm.hp = 9999;
            }
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK && col.name.Contains("Slash"))
                {
                    if (SetupMode)
                        Remove();
                }
            }

        }
        [Description("开关，注意和门对应编号。\n当然，你要多个开关对应一个门我也不拦着你")]
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
                int gateNum = ((ItemDef.LeverGateItem)item).Number;
                var gateName = $"{ItemDef.LeverGateItem.GateNamePrefix}{gateNum}";
                playMakerFSM = gameObject.LocateMyFSM("toll switch");
                playMakerFSM.GetAction<FindGameObject>("Initiate", 2).objectName = gateName;
            }
        }

        [Description("由拉杆开关触发的门，注意编号对应。\n一个门可以有多个开关，但是一个开关只能开一个门")]
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
                int gateNum = ((ItemDef.LeverGateItem)item).Number;
                var gateName = $"{ItemDef.LeverGateItem.GateNamePrefix}{gateNum}";
                gameObject.name = gateName;
            }
        }

        [Description("危险重生点，你可以理解为存档点")]
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

        
        [Description("看起来能破坏的墙壁")]
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
        [Description("看起来能破坏，但其实不可破坏的墙壁")]
        [Decoration("HK_unbreak_wall")]
        public class UnBreakWall : Resizeable
        {
            private void Awake()
            {
                var bw = Instantiate(ObjectLoader.InstantiableObjects["HK_break_wall"]);
                var fsm = bw.GetComponent<PlayMakerFSM>();
                if(fsm)
                {
                    fsm.RemoveTransition("Initiate", "ACTIVATE");
                    fsm.RemoveAction("Initiate", 11);
                    fsm.SetState("Pause");
                    fsm.enabled = false;
                    Destroy(fsm);
                }
                Destroy(bw.GetComponent<CustomDecoration>());
                bw.transform.SetParent(gameObject.transform);
                bw.transform.localPosition = Vector3.zero;
                UnVisableBehaviour.AttackReact.Create(gameObject);
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
