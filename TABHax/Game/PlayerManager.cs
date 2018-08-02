using Landfall.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TABHax.Scripts;
using UnityEngine;

namespace TABHax.Game
{
    public static class PlayerManager
    {
        public static Player[] AllPlayers { get; private set; }
        public static Player[] AlivePlayers { get; private set; }
        public static Player[] LookingAtMe { get; private set; }
        public static Player[] Visible { get; private set; }
        public static Player Nearest { get; private set; }
        public static Player LocalPlayer { get { return Player.localPlayer; } }
        public static Player Target { get { return RaycastForPlayerEx2(LocalPlayer.GetCameraTransform()); } }

        public static void Update()
        {
            AllPlayers = UnityEngine.Object.FindObjectsOfType<Player>() ?? new Player[0];
            AlivePlayers = new Player[0];

            Nearest = null;

            if (AllPlayers.Length > 0)
            {
                if (LocalPlayer)
                    Nearest = LocalPlayer.GetNearestPlayer(AllPlayers);

                var alive = new List<Player>(AllPlayers.Length);
                foreach (var p in AllPlayers)
                    if (p.IsAlive() && p != LocalPlayer)
                        alive.Add(p);
                AlivePlayers = alive.ToArray();
            }

            var looking = new List<Player>();
            foreach(var p in AlivePlayers)
                if (Physics.Raycast(p.GetCameraTransform().position, p.GetCameraTransform().forward.normalized, 500f, (int)PlayerManager.Layers.LocalPlayer))
                    looking.Add(p);
            LookingAtMe = looking.ToArray();

            var visible = new List<Player>();
            foreach(var p in AlivePlayers)
                if (PlayerVisible(LocalPlayer.GetCameraTransform(), p))
                    visible.Add(p);
            Visible = visible.ToArray();
        }

        public enum Layers
        {
            Default = 1 << 0,
            TransparentFX = 1 << 1,
            IgnoreRaycast = 1 << 2,
            __unknown0 = 1 << 3,
            Water = 1 << 4,
            UI = 1 << 5,
            __unknown1 = 1 << 6,
            __unknown2 = 1 << 7,
            HideFromSelf = 1 << 8,
            Map = 1 << 9,
            Stickys = 1 << 10,
            Props = 1 << 11,
            Terrain = 1 << 12,
            LocalPlayer = 1 << 13,
            Weapon01 = 1 << 14,
            Weapon02 = 1 << 15,
            Weapon03 = 1 << 16,
            ScreenParticles = 1 << 17,
            Road = 1 << 18,
            Wheel = 1 << 19,
            DontRender = 1 << 20,
            DontCollide = 1 << 21,
            DontCollideWithLocalPlayer = 1 << 22,
            Dropper = 1 << 23,
            DroppedRagdoll = 1 << 24,
            Armor = 1 << 25,
            Helmet = 1 << 26,
            IgnoreBullets = 1 << 27,
            __unknown3 = 1 << 28,
            __unknown4 = 1 << 29,
            __unknown5 = 1 << 30,
            __unknown6 = 1 << 31,
            //For Raycasting
            RaycastPlayers = ~(LocalPlayer | Weapon01 | Weapon02 | Weapon03 | DontCollide | Armor | Helmet | IgnoreBullets)
        }

        public static bool RaycastForPlayer(Transform from)
        {
            return RaycastForPlayer(from.position, from.forward.normalized);
        }
        public static bool RaycastForPlayer(Vector3 position, Vector3 direction)
        {
            return Physics.Raycast(position, direction, 9999f, (int)Layers.RaycastPlayers);
        }

        public static bool RaycastForPlayerEx(Transform from, out RaycastHit hit)
        {
            return RaycastForPlayerEx(from.position, from.forward.normalized, out hit);
        }
        public static bool RaycastForPlayerEx(Vector3 position, Vector3 direction, out RaycastHit hit)
        {
            return Physics.Raycast(position, direction, out hit, 999f, (int)Layers.RaycastPlayers);
        }

        public static Player RaycastForPlayerEx2(Transform from)
        {
            return RaycastForPlayerEx2(from.position, from.forward.normalized);
        }
        public static Player RaycastForPlayerEx2(Vector3 position, Vector3 direction)
        {
            RaycastHit hit;
            if(RaycastForPlayerEx(position,direction, out hit))
                return hit.transform.GetComponentInParent<Player>();
            return null;
        }

        public static bool PlayerVisible(Transform from, Player p)
        {
            var delta = (p.m_head.transform.position - from.position - from.forward * -1f);
            var dir = delta.normalized;
            return !Physics.Raycast(Player.localPlayer.m_playerCamera.transform.position, dir, delta.magnitude, (int)Layers.RaycastPlayers);
        }

        public static Player GetPlayerById(byte id)
        {
            foreach (var p in AlivePlayers)
                if (p && p.GetID() == id)
                    return p;
            return null;
        }

        public static Lighter GetLighter(this Player player)
        {
            return player.gameObject.GetComponent<Lighter>() ?? player.gameObject.AddComponent<Lighter>();
        }
    }
}
