using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Patches
{
    [HarmonyPatch(typeof(Gun), "Shoot", null)]
    public static class ShootPatch
    {
        public static bool InfiniteAmmo { get; set; } = true;
        public static bool NoSpread { get; set; } = true;
        public static bool AutoFire { get; set; } = true;
        public static bool InstantFire { get; set; } = true;


        // Token: 0x06000090 RID: 144 RVA: 0x000077C0 File Offset: 0x000059C0
        private static void Prefix(Gun __instance)
        {
            if (InfiniteAmmo)
            {
                __instance.bullets = 10000;
                __instance.bulletsInMag = 999;
            }
            if (NoSpread)
            {
                __instance.extraSpread = 0f; RandomizeRotation component = __instance.gameObject.GetComponent<RandomizeRotation>();
                if (component)
                    __instance.gameObject.GetComponent<RandomizeRotation>().spread = 0f;
            }
            if (AutoFire)
                __instance.currentFireMode = 2;
            if (InstantFire)
                __instance.rateOfFire = 1E-06f;

            var projHit = __instance.GetComponentInChildren<ProjectileHit>();
            if (projHit)
            {
                projHit.force = 9999f;
            }
        }
    }
}
