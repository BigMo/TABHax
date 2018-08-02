using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABHax.UI;
using UnityEngine;

namespace TABHax
{
    public class Loader
    {
        public static Hax HaxInstance;
        public static ESP ESPInstance;
        public static TABUI UIInstance;

        public static void Init()
        {
            Loader.Load = new GameObject();
            HaxInstance = Loader.Load.AddComponent<Hax>();
            ESPInstance = Loader.Load.AddComponent<ESP>();
            UIInstance = Loader.Load.AddComponent<TABUI>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

        private static GameObject Load;
    }
}
