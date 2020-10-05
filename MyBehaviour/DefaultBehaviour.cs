using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecorationMaster.Util;
using DecorationMaster.Attr;
using UnityEngine;
using ModCommon;
namespace DecorationMaster.MyBehaviour
{
    [Decoration("HK_trap_spike")]
    [Decoration("HK_flip_platform")]
    [Decoration("HK_spike")]
    [Decoration("HK_platform_rect")]
    [Decoration("HK_soul_totem")]
    [Decoration("HK_crystal_barrel")]
    [Decoration("HK_platform_small")]
    [Decoration("HK_crystal")]
    [Decoration("HK_bounce_shroom")]
    [Decoration("HK_stomper")]
    [Decoration("HK_lazer_bug")]
    
    //[Decoration("HK_conveyor")]
    public class DefaultBehaviour : Resizeable
    {
        [Serializable]
        public class SharedItem : ResizableItem
        {
        }
    }

    
    public class UnVisableBehaviour : CustomDecoration
    {
        [Serializable]
        public class SharedItem : Item
        {
        }
        private class AttackReact : MonoBehaviour
        {
            public CustomDecoration parent;
            private void Awake()
            {
                gameObject.AddComponent<NonBouncer>();
                gameObject.AddComponent<BoxCollider2D>().size = Vector2.one;
                if(ItemManager.Instance.setupMode)
                    gameObject.AddComponent<ShowColliders>();
            }
            public void OnTriggerEnter2D(Collider2D col)
            {
                if (col.gameObject.layer == (int)GlobalEnums.PhysLayers.HERO_ATTACK)
                {
                    if (ItemManager.Instance.setupMode)
                        parent?.Remove();
                }
            }
        }
        public ShowColliders colDisp;
        public void Awake()
        {
            var child = new GameObject();
            child.transform.SetParent(gameObject.transform);
            child.layer = (int)GlobalEnums.PhysLayers.PROJECTILES;
            child.AddComponent<AttackReact>().parent = this;
            
            gameObject.AddComponent<NonBouncer>();
            if(SetupMode)
                colDisp = gameObject.AddComponent<ShowColliders>();
            Logger.LogDebug("Awake Unvisable");
            
        }
        
    }
}
