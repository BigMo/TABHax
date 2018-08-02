using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(PositionShake), "AddShakeWorld", null)]
    public static class NoShake3
    {
        public static bool Active { get; set; } = true;


        // Token: 0x0600008F RID: 143 RVA: 0x000077A0 File Offset: 0x000059A0
        private static bool Prefix()
        {
            return !Active;
        }
    }
}
