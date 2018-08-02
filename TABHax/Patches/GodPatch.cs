using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(PlayerDeath), "OMIBLMPLBBE", null)]
    public static class GodPatch
    {
        // Token: 0x06000091 RID: 145 RVA: 0x00007848 File Offset: 0x00005A48
        private static bool Prefix(PlayerDeath __instance)
        {
            //Console.WriteLine("{0} TAKES DAMAGE", __instance.transform.parent.name);
            if (__instance == Player.localPlayer.m_playerDeath)
                return false;
            return true;
        }
    }
}
