using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(Recoil), "AddRecoil", null)]
    public static class NoRecoil
    {
        public static bool Active { get; set; } = true;


        // Token: 0x0600008E RID: 142 RVA: 0x00007780 File Offset: 0x00005980
        private static bool Prefix()
        {
            return !Active;
        }
    }
}
