using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TABHax.Patches
{
    public class Patch
    {
        public static void DoPatches()
        {
            var h = HarmonyInstance.Create("meh");
            h.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
