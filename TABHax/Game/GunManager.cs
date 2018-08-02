using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Game
{
    public static class GunManager
    {
        public static Gun LeftGun { get { return Player.localPlayer?.m_weaponHandler?.leftWeapon?.gun; } }
        public static Gun RightGun { get { return Player.localPlayer?.m_weaponHandler?.rightWeapon?.gun; } }

        public static Player NearestFov
        {
            get
            {
                var cam = Player.localPlayer.m_playerCamera.GetComponent<Camera>();
                Player closest = null;
                var closestDist = float.MaxValue;
                var screenMid = new Vector2(Screen.width, Screen.height);

                foreach (var p in PlayerManager.AlivePlayers)
                {
                    var delta = (p.m_head.transform.position - Player.localPlayer.m_playerCamera.transform.position - Player.localPlayer.m_playerCamera.transform.forward * -1f);
                    var dir = delta.normalized;
                    var visible = !Physics.Raycast(Player.localPlayer.m_playerCamera.transform.position, dir, delta.magnitude, ~1);
                    if (!visible)
                        continue;

                    var w2s = cam.WorldToScreenPoint(p.m_head.transform.position);
                    if (w2s.z < 0)
                        continue;

                    var deltaDist = (new Vector2(w2s.x, Screen.height - w2s.y) - screenMid).magnitude;
                    if (deltaDist < closestDist)
                    {
                        closestDist = deltaDist;
                        closest = p;
                    }
                }

                return closest;
            }
        }

        public static void AimAt(Gun gun, Transform trans)
        {
            if (gun && trans)
            {
                gun.transform.LookAt(trans);
                gun.GetField<Transform>("LMFIBANOEHH").LookAt(trans);
            }
        }
    }
}
