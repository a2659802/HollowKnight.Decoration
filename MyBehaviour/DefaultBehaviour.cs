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
    [Decoration("HK_lazer_bug")]
    [Decoration("HK_crystal_barrel")]
    [Decoration("HK_platform_small")]
    [Decoration("HK_crystal")]
    [Decoration("HK_bounce_shroom")]
    [Decoration("HK_stomper")]
    //[Decoration("HK_conveyor")]
    public class DefaultBehaviour : Resizeable
    {
        [Serializable]
        public class SharedItem : ResizableItem
        {
        }
    }

    [Decoration("HK_respawn_point")]
    public class UnVisableBehaviour : CustomDecoration
    {
        [Serializable]
        public class SharedItem : Item
        {
        }

        public ShowColliders colDisp;
        public void Awake()
        {
            gameObject.layer = (int)GlobalEnums.PhysLayers.ENEMIES;
            gameObject.AddComponent<NonBouncer>();
            colDisp = gameObject.AddComponent<ShowColliders>();
            Logger.LogDebug("Awake Unvisable");
            
        }
        public void OnDestroy()
        {
            Logger.LogDebug("DIE Unvisiable");
        }

    }
}
