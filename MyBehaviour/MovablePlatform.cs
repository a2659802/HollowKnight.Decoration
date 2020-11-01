using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DecorationMaster.Attr;

using DecorationMaster.UI;
using DecorationMaster.MyBehaviour;
namespace DecorationMaster.MyBehaviour
{
    public class MovablePlatform
    {
        [AdvanceDecoration]
        [Description("可移动的翻转平台,\n有显示上的BUG，暂时没有修复方案")]
        [Description("movable platform, there an issue in it, do not use", "en-us")]
        [Decoration("move_flip_platform")]
        public class MoveFilp : SawMovement
        {
            private GameObject plat;
            private void Awake()
            {
                if (ObjectLoader.InstantiableObjects.TryGetValue("HK_flip_platform", out var flip))
                {
                    
                    var fgo = Instantiate(flip);
                    Destroy(fgo.GetComponent<DefaultBehaviour>());
                    fgo.transform.SetParent(gameObject.transform);
                    fgo.transform.localPosition = Vector3.zero;
                    fgo.SetActive(true);
                    fgo.transform.Find("Spikes Bottom").GetComponent<DamageHero>().hazardType = (int)GlobalEnums.HazardType.ACID;
                    fgo.transform.Find("Spikes Top").GetComponent<DamageHero>().hazardType = (int)GlobalEnums.HazardType.ACID;
                    plat = fgo;
                    UnVisableBehaviour.AttackReact.Create(gameObject);
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
            public override void Hit(HitInstance damageInstance)
            {
                ;
            }
        }

        [Description("闪烁平台，会不断消失-出现")]
        [Description("twinkle platform \n will continue appear-disappear loop,you can set the frequence ", "en-us")]
        [Decoration("twinkle_platform")]
        public class TempPlatform : Resizeable
        {
            private float dt = 0;
            private SpriteRenderer sr;
            private BoxCollider2D col;
            private void Awake()
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
                var tex = GUIController.Instance.images["seal_wall"];
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                col = gameObject.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1, 4.5f);
                gameObject.layer = 8;
                UnVisableBehaviour.AttackReact.Create(gameObject);

            }
            private void Update()
            {
                var maxt = ((ItemDef.TempPlatItem)item).Time;
                dt = (Time.realtimeSinceStartup + ((ItemDef.TempPlatItem)item).Offset * maxt * 0.5f) % maxt;
                //dt = Time.d
                //dt %= maxt;
                float a = (maxt-dt) / maxt;
                if(a<0.5f)
                {
                    sr.color = new Color(1, 0.8f, 0.8f, a);
                    col.enabled = false;
                }
                else
                {
                    sr.color = new Color(1, 1, 1, a);
                    col.enabled = true;
                }
            }
        }

        [MemeDecoration]
        [Description("可移动的平台")]
        [Description("movable platform, there an issue in it, do not use", "en-us")]
        [Decoration("mary_move_platform")]
        public class MoveSuperMary : SawMovement
        {
            private SpriteRenderer sr;
            private BoxCollider2D col;
            private void Awake()
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
                var tex = GUIController.Instance.images["plat_supermary"];
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                col = gameObject.AddComponent<BoxCollider2D>();
                col.size = new Vector2(0.7f, 0.045f);
                gameObject.layer = 8;
                col.sharedMaterial = new PhysicsMaterial2D { bounciness = 0, friction = 1 };
            }
            public override Vector3 Move(Vector3 current)
            {
                var sitem = item as ItemDef.SawItem;

                float dealtDis = sitem.span * Mathf.Sin(sitem.speed * Time.time + sitem.offset * Mathf.PI / 2);
                float dx = dealtDis * Mathf.Cos(sitem.angle * Mathf.PI / 180f);
                float dy = dealtDis * Mathf.Sin(sitem.angle * Mathf.PI / 180f);
                return new Vector3(sitem.Center.x + dx, sitem.Center.y + dy, current.z);
            }
            public override void HandleSize(float size)
            {
                base.HandleSize(size * 3f);
            }
        }
    }
}
