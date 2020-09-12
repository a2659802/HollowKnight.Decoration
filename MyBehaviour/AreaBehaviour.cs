using DecorationMaster.Attr;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public class BindLantern: BreakableBoolBinding
        {
            public override string BindBoolValue => nameof(PlayerData.hasLantern);
        }

    }

}
