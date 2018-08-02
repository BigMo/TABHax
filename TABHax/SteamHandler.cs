using Landfall.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TABHax.Utils;

namespace TABHax
{
    public static class SteamHandler
    {
        private static PropertyInfo STEAM_NAME = typeof(PhotonServerConnector).GetProperty("HHLCJCDEKGL", BindingFlags.Static | BindingFlags.Public);
        private static FieldInfo STEAM_ID_HOLDER = typeof(PhotonServerConnector).GetField("ICGCBJECCPA", BindingFlags.Static | BindingFlags.NonPublic);
        private static PropertyInfo STEAM_ID_FIELD = typeof(KDAOBBIDPKB).GetProperty("BAKNHHHBDJP", BindingFlags.Instance | BindingFlags.Public);

        public static string CurrentName { get; private set; }
        public static ulong CurrentID { get; private set; }

        private static KDAOBBIDPKB holder;

        public static void Update()
        {
            if(STEAM_NAME != null)
                CurrentName = (string)STEAM_NAME.GetValue(null, null);

            holder = (KDAOBBIDPKB)STEAM_ID_HOLDER.GetValue(null);
            if (holder != null)
            {
                var id = (CSteamID)STEAM_ID_FIELD.GetValue(holder, null);
                CurrentID = id.m_SteamID;
            }
        }

        public static void FuckUpSteamData()
        {
            if (STEAM_NAME == null || STEAM_ID_HOLDER == null || STEAM_ID_FIELD == null)
            {
                Console.WriteLine("ERROR: {0} {1} {2}", STEAM_NAME != null, STEAM_ID_HOLDER != null, STEAM_ID_FIELD != null);
                return;
            }

            STEAM_NAME.SetValue(null, RandomWrapper.GenerateString(8), null);

            if (holder != null)
            {
                var id = (CSteamID)STEAM_ID_FIELD.GetValue(holder, null);
                var oldId = id.m_SteamID;
                id = new CSteamID(new AccountID_t((uint)UnityEngine.Random.Range(0, int.MaxValue)), id.GetEUniverse(), id.GetEAccountType());
                CurrentID = id.m_SteamID;
                STEAM_ID_FIELD.SetValue(holder, id, null);
            }
        }
    }
}
