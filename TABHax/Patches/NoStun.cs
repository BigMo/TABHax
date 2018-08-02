using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(StunEffect), "Stun", null)]
    public static class NoStun
    {
        public static bool Active { get; set; } = true;


        // Token: 0x06000091 RID: 145 RVA: 0x00007848 File Offset: 0x00005A48
        private static bool Prefix(PlayerDeath __instance)
        {
            return !Active;
        }
    }
}
