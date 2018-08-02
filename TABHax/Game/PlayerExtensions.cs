using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Game
{
    public static class PlayerExtensions
    {
        public static byte GetID(this Player player)
        {
            var np = player.GetComponent<Landfall.Network.NetworkPlayer>();
            if (!np)
                return 0;
            return np.GetField<byte>("OCDCPIGJOLN");
        }

        public static Transform GetCameraTransform(this Player player)
        {
            if (player.m_playerCamera)
                return player.m_playerCamera.transform;
            return null;
        }

        public static bool IsAlive(this Player player)
        {
            if (player.m_playerDeath)
                return !player.m_playerDeath.dead;
            return false;
        }

        public static bool IsVisibleFrom(this Player player, Vector3 position)
        {
            var dir = (player.GetHeadTransform().position - position).normalized;
            return PlayerManager.RaycastForPlayer(position, dir);
        }

        public static Transform GetHeadTransform(this Player player)
        {
            if (player.m_head)
                return player.m_head.transform;
            return null;
        }

        public static bool IsValid(this Player player)
        {
            if (!player)
                return false;

            if (!player.m_playerDeath || !player.m_playerDeath.dead)
                return false;

            return true;
        }

        public static Player GetNearestPlayer(this Player from, Player[] to)
        {
            float lowestDistance = float.MaxValue;
            Player target = null;
            foreach (Player p in to)
            {
                if (p == from)
                    continue;

                if (!IsValid(p))
                    continue;

                float distance = (p.GetHeadTransform().position - from.GetHeadTransform().position).sqrMagnitude;
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    target = p;
                }

            }
            return target;
        }

        public static void Kill(this Player p)
        {
            p.headDamagable.NetworkDamage(100);
            p.bodyDamagable.NetworkDamage(100);
            p.m_playerDeath.health = 0;
            p.m_playerDeath.DamagePlayer(100, p.transform.position);
            p.m_playerDeath.TakeDamage(p.transform.position * 999, p.transform.position * 999);
        }
    }
}
