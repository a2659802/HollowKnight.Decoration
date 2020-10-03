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
        [Decoration("IMG_MothwingCloak")]
        public class BindDash: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.canDash);
        }

        [Decoration("IMG_MonarchWings")]
        public class BindDoubleJump: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.hasDoubleJump);
        }

        [Decoration("IMG_MantisClaw")]
        public class BindClaw: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.hasWalljump);
        }

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
