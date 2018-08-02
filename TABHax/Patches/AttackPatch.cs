using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(MeleeWeapon), "Attack", null)]
    public static class AttackPatch
    {
        public static bool Active { get; set; } = true;

        // Token: 0x0600008D RID: 141 RVA: 0x00007738 File Offset: 0x00005938
        private static void Prefix(MeleeWeapon __instance)
        {
            if (Active && __instance != null)
            {
                    __instance.damageOnHit = 99999f;
                    __instance.forceOnHit = 5000f;
                    __instance.cd = 0f;
            }
        }
    }
}
