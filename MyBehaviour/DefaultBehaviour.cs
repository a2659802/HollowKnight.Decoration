using System;
using DecorationMaster.Util;
using DecorationMaster.Attr;
using UnityEngine;
using ModCommon;
using System.Collections;

namespace DecorationMaster.MyBehaviour
{
    [Description("这是游戏内原本的物品，基本没有任何改动，看图标识别物品")]
    [Description("This is the origin game object, never changed its function")]
    [Decoration("HK_flip_platform")]
    [Decoration("HK_spike")]
    [Decoration("HK_platform_rect")]
    [Decoration("HK_soul_totem")]
    [Decoration("HK_crystal_barrel")]
    [Decoration("HK_platform_small")]
    [Decoration("HK_crystal")]
    [Decoration("HK_bounce_shroom")]
    [Decoration("HK_stomper")]
    [Decoration("HK_zote_head")]
    [Decoration("HK_infinte_soul")]

    [Decoration("HK_laser_turret")]
    [Decoration("HK_quake_floor")]

    public class DefaultBehaviour : Resizeable
    {
        [Serializable]
        public class SharedItem : ResizableItem
        {
            
        }
    }

    [Decoration("HK_trap_spike")]
    [Decoration("HK_zap_cloud")]
    public class DelayResizableBehaviour : Resizeable
    {
        [Serializable]
        public class DelayResizeItem : ResizableItem
        {
            [Handle(Operation.SetTimeOffset)]
            [FloatConstraint(0f, 2f)]
            public float time_offset { get; set; } = 0;
        }

        public override void HandleInit(Item i)
        {
            try
            {
                GameManager.instance.StartCoroutine(DelaySpawn(((DelayResizeItem)i).time_offset));
            }
            catch
            {
                base.HandleInit(i);
            }
            IEnumerator DelaySpawn(float t)
            {
                yield return new WaitForSeconds(t);
                base.HandleInit(i);
            }
               
        }
    }
    
    public class UnVisableBehaviour : CustomDecoration
    {
        [Serializable]
        public class SharedItem : Item
        {
        }
        public class AttackReact : MonoBehaviour
        {
            public CustomDecoration parent;
            private void Awake()
            {
                gameObject.AddComponent<NonBouncer>();
                gameObject.AddComponent<BoxCollider2D>().size = Vector2.one * 2;
                if(ItemManager.Instance.setupMode)
                    gameObject.AddComponent<ShowColliders>();
            }
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK && col.name.Contains("Slash"))
                {
                    if (ItemManager.Instance.setupMode)
                        parent?.Remove();
                }
            }
            public static void Create(GameObject parent)
            {
                var child = new GameObject();
                child.name = "UnvisableAttackHelper";
                child.transform.SetParent(parent.transform);
                child.layer = (int)GlobalEnums.PhysLayers.PROJECTILES;
                child.AddComponent<AttackReact>().parent = parent.GetComponent<CustomDecoration>();
                child.transform.localPosition = Vector3.zero;
            }
              
        }
        public ShowColliders colDisp;
        public void Awake()
        {
            AttackReact.Create(gameObject);
        }
        
    }
}
