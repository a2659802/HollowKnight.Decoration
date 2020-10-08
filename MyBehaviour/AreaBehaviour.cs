using DecorationMaster.Attr;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DecorationMaster.MyBehaviour
{
    public class AreaBehaviour
    {
        [Description("禁用能力-冲刺。\n非编辑模式下可用攻击暂时移除\n如果你不想被移除，那就放到打不到的地方")]
        [Decoration("IMG_MothwingCloak")]
        public class BindDash: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.canDash);
        }
        [Description("禁用能力-二段跳\n非编辑模式下可用攻击暂时移除\n如果你不想被移除，那就放到打不到的地方")]
        [Decoration("IMG_MonarchWings")]
        public class BindDoubleJump: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.hasDoubleJump);
        }
        [Description("禁用能力-爬墙\n非编辑模式下可用攻击暂时移除\n如果你不想被移除，那就放到打不到的地方")]
        [Decoration("IMG_MantisClaw")]
        public class BindClaw: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.hasWalljump);
        }
        [Description("设置黑暗：每个灯笼1级黑暗，最多2级\n非编辑模式下可用攻击暂时移除\n如果你不想被移除，那就放到打不到的地方")]
        [Decoration("IMG_Lantern")]
        public class BindLantern: Resizeable
        {
            private static int _dark;
            private static int darknessLevel { 
                get => _dark; 
                set
                {
                    if (value > 2)
                        _dark = 2;
                    else if (value < 0)
                        _dark = 0;
                    else
                        _dark = value;
                } 
            }
            private bool noLantern;
            public override void Hit(HitInstance hit)
            {
                base.Hit(hit);
                Destroy(gameObject);
            }
            private void OnEnable()
            {
                noLantern = true;
                darknessLevel++;
                UpdateDark();
            }
            private void OnDisable()
            {
                noLantern = false;
                darknessLevel--;
                UpdateDark();
            }
            private void UpdateDark()
            {
                if (DecorationMaster.GM && DecorationMaster.GM.IsGameplayScene())
                {
                    GameObject go = GameObject.FindGameObjectWithTag("Vignette");
                    if (go)
                    {
                        PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(go, "Darkness Control");
                        if (playMakerFSM)
                        {
                            FSMUtility.SetInt(playMakerFSM, "Darkness Level", darknessLevel);
                        }
                        if (!this.noLantern)
                        {
                            FSMUtility.LocateFSM(go, "Darkness Control").SendEvent("RESET");
                        }
                        else
                        {
                            FSMUtility.LocateFSM(go, "Darkness Control").SendEvent("SCENE RESET NO LANTERN");
                            if (HeroController.instance != null)
                            {
                                HeroController.instance.wieldingLantern = false;
                            }
                        }
                    }
                }
            }
        }

    }

}
