using HighlightingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TABHax.Game;
using UnityEngine;

namespace TABHax.UI
{
    public class ESP : MonoBehaviour
    {
        public bool CfgDrawPlayers { get; set; } = true;
        public float CfgPlayerDistance { get; set; } = 10000f;
        public float CfgPickupDistance { get; set; } = 50f;
        public bool CfgDrawWeapons { get; set; } = true;
        public bool CfgDrawAttachments { get; set; } = true;
        public bool CfgDrawArmor { get; set; } = true;
        public bool CfgDrawHealth { get; set; } = true;
        public bool CfgDrawOthers { get; set; } = true;


        private static FieldInfo gunTransformField = typeof(Gun).GetField("LMFIBANOEHH", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        private GameObject pointer;

        public void Start()
        {
            pointer = new GameObject();
            var pt = pointer.AddComponent<Light>();
            UnityEngine.Object.DontDestroyOnLoad(pointer);
            pt.type = LightType.Spot;
            pt.color = Color.green;
        }

        private void DrawCrosshairs()
        {
            var cam = Player.localPlayer.m_playerCamera.GetComponent<Camera>();
            RaycastHit hit;

            //Guns
            if (Player.localPlayer.m_weaponHandler && (Player.localPlayer.m_weaponHandler.rightWeapon || Player.localPlayer.m_weaponHandler.leftWeapon))
            {
                var CurSlot = Player.localPlayer.m_weaponHandler.LJPNCHJMEAI; //get current holding slot
                var CurrentGun = Player.localPlayer.m_inventory.IJIKCMPPFOA(CurSlot).GetComponent<Pickup>().PHDMFNFACCC; //get the gun we hold
                if (CurrentGun)
                {
                    Transform gunTransform = (Transform)gunTransformField.GetValue(CurrentGun);
                    var dir = gunTransform.forward.normalized;
                    if (Physics.Raycast(gunTransform.position, dir, out hit))
                        DrawCrosshair(cam, hit.point, Color.red);
                    else
                        DrawCrosshair(cam, gunTransform.position + dir * 9999, Color.red);
                }
            }

            if (Physics.Raycast(Player.localPlayer.m_playerCamera.transform.position, Player.localPlayer.m_playerCamera.transform.forward.normalized, out hit))
            {
                pointer.SetActive(true);
                pointer.transform.position = hit.point + Vector3.up * 2f;
                pointer.transform.LookAt(hit.point);
                
                DrawCrosshair(cam, hit.point, Color.green);
            }
            else
            {
                pointer.SetActive(false);
                DrawCrosshair(cam, Player.localPlayer.m_playerCamera.transform.position + Player.localPlayer.m_playerCamera.transform.forward * 9999, Color.gray);
            }

            var target = PlayerManager.Target;
            if (target)
            {
                ZatsRenderer.DrawString(new Vector2(Screen.width / 2f, Screen.height / 2f),
                    string.Format("{0} ({1}hp)", target.name, (int)target.m_playerDeath.health),
                    Color.red, false);
            }
        }
        private void DrawCrosshair(Camera cam, Vector3 t, Color color)
        {
            var vec = cam.WorldToScreenPoint(t);
            if(vec.z > 0)
                ZatsRenderer.DrawCross(new Vector2(vec.x, vec.y), Vector2.one * 8f, 1f, color);
        }

        private void DrawPlayers()
        {
            if (!CfgDrawPlayers)
                return;

            var cam = Player.localPlayer.m_playerCamera.GetComponent<Camera>();
            foreach (var p in PlayerManager.AlivePlayers)
            {
                if (p != Player.localPlayer)
                {
                    if (!DrawPlayer(p, cam))
                    {
                        SetHighlighter(p.gameObject, Color.red, false);
                        p.GetLighter().Disable();
                    }
                }
            }

            var vec = new Vector2(4, Screen.height / 2f);
            foreach (var p in PlayerManager.LookingAtMe)
            {
                if (p != Player.localPlayer)
                {
                    ZatsRenderer.DrawString(vec, p.name, Color.red, false);
                    vec += Vector2.down * 18f;
                }
            }
        }
        private void SetHighlighter(GameObject o, Color color, bool on)
        {
            if (!o)
                return;

            var h = o.GetComponent<Highlighter>();
            if (!h)
                h = o.AddComponent<Highlighter>();

                if (on)
                    h.ConstantOnImmediate(color);
                else
                    h.ConstantOffImmediate();
        }
        private bool DrawPlayer(Player p, Camera cam)
        {
            var health = p.m_playerDeath.health;
            if (health <= 0)
                return false;

            var vec2 = p.m_torso.transform.position - new Vector3(0f, 1.4f, 0f);
            var dist = (int)Vector3.Distance(Player.localPlayer.m_torso.transform.position, vec2);
            if (dist > CfgPlayerDistance)
                return false;


            var vec = cam.WorldToScreenPoint(vec2);
            if (vec.z < 0)
                return false;

            var delta = (p.m_head.transform.position - Player.localPlayer.m_playerCamera.transform.position - Player.localPlayer.m_playerCamera.transform.forward * -1f);
            var dir = delta.normalized;
            var visible = !Physics.Raycast(Player.localPlayer.m_playerCamera.transform.position, dir, delta.magnitude, ~1);

            var vec3 = cam.WorldToScreenPoint(vec2 + new Vector3(0f, 2.25f, 0f));
            var height = Mathf.Abs(vec.y - vec3.y);
            var width = height / 2f;

            var color = Color.red;
            if (!visible)
                color = Color.yellow;
            SetHighlighter(p.gameObject, color, true);
            p.GetLighter().Color = color;
            color.a = ClampColor(dist, CfgPlayerDistance, CfgPlayerDistance/10f, 1f,0.1f);
            //ZatsRenderer.DrawBox(new Vector2(vec3.x - width / 2f, Screen.height - vec3.y), new Vector2(width, height), 1f, color);
            ZatsRenderer.DrawString(new Vector2(vec3.x, Screen.height - vec3.y), string.Format("{0} | {1}m\n{2} hp", p.name, dist, (int)health), color,true);
            return true;
        }
        private static Color[] equipColors = new Color[]
                    {
                        Color.black,
                        Color.white,
                        Color.blue,
                        Color.green,
                        Color.magenta
                    };

        private void DrawPickups()
        {
            var pickups = PickupManager.instance.m_Pickups;
            var cam = Player.localPlayer.m_playerCamera.GetComponent<Camera>();
            foreach (var p in pickups)
            {
                if(DrawPickup(p, cam))
                    SetHighlighter(p.gameObject, GetColorOfPickup(p), true);
                else
                    SetHighlighter(p.gameObject, Color.red, false);
            }
        }
        private bool DrawPickup(Pickup p, Camera cam)
        {
            var dist = Vector3.Distance(Player.localPlayer.m_torso.transform.position, p.transform.position);
            if (!p.canInteract)
                return false;

            if (dist > CfgPickupDistance)
                return false;

            var vec = cam.WorldToScreenPoint(p.transform.position);
            if (vec.z < 0)
                return false;

            var size = 60f / dist;
            switch (p.weaponType)
            {
                case Pickup.JGHOAEDPDBB.Weapon:
                    if (!CfgDrawWeapons)
                        return false;
                    break;
                case Pickup.JGHOAEDPDBB.WeaponAttatchment:
                    if (!CfgDrawAttachments)
                        return false;
                    break;
                case Pickup.JGHOAEDPDBB.Armor:
                    if (!CfgDrawArmor)
                        return false;
                    break;
                case Pickup.JGHOAEDPDBB.Health:
                    if (!CfgDrawHealth)
                        return false;
                    break;
                default:
                    if (!CfgDrawOthers)
                        return false;
                    break;
            }

            var color = GetColorOfPickup(p);
            color.a = ClampColor(dist, CfgPickupDistance, CfgPickupDistance/10f);

            //ZatsRenderer.DrawBox(new Vector2(vec.x - size / 2f, Screen.height - vec.y), new Vector2(size,size), 1f, color);
            ZatsRenderer.DrawString(new Vector2(vec.x, Screen.height - vec.y), string.Format("{0}\n{1}", p.name.Replace("(Clone)", "").Trim(), p.weaponType), color, true);
            return true;
        }
        public static Color GetColorOfPickup(Pickup p)
        {
            var color = Color.white;
            switch (p.weaponType)
            {
                case Pickup.JGHOAEDPDBB.Weapon:
                    color = equipColors[0];
                    break;
                case Pickup.JGHOAEDPDBB.WeaponAttatchment:
                    color = equipColors[1];
                    break;
                case Pickup.JGHOAEDPDBB.Armor:
                    color = equipColors[2];
                    break;
                case Pickup.JGHOAEDPDBB.Health:
                    color = equipColors[3];
                    break;
                default:
                    color = equipColors[3];
                    break;
            }
            return color;
        }

        private void DrawRadar()
        {
            var local = Player.localPlayer.m_playerCamera;
            var mapPos = Vector2.one * 4f;
            var mapSize = Vector2.one * 128f;
            var mapCenter = mapPos + mapSize * 0.5f;
            var maxDist = 100f;

            ZatsRenderer.DrawCross(mapPos + mapSize * 0.5f, mapSize, 1f, Color.white);
            ZatsRenderer.DrawDot(mapPos + mapSize * 0.5f, Color.green);
            
            if (PlayerManager.AlivePlayers.Length == 0)
                return;

            var world2radar = (mapSize * 0.5f).magnitude / maxDist;
            foreach(var p in PlayerManager.AlivePlayers)
            {
                var delta = p.m_torso.transform.position - local.transform.position;

                if (delta.magnitude > maxDist)
                    delta = delta.normalized * maxDist;
                delta *= world2radar;

                if (delta.magnitude > mapSize.x * 0.5f)
                    delta = delta.normalized * mapSize.x * 0.5f;

                var length = delta.magnitude;
                var angle = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg + 90f;
                var newAngle = (angle + local.transform.rotation.eulerAngles.y) * -1f;
                var newVec = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * newAngle), Mathf.Cos(Mathf.Deg2Rad * newAngle)) * length;

                ZatsRenderer.DrawDot(mapCenter + newVec, Color.red);
            }
        }

        private float ClampColor(float dist, float maxDist, float minDist, float maxAlpha = 1f, float minAlpha = 0f)
        {
            float a = 0f;
            if (dist <= minDist)
                a = 1f;
            else if (dist >= maxDist)
                a = 0f;
            else
                a = 1f - ((dist - minDist) / (maxDist - minDist));

            return Mathf.Clamp(a, minAlpha, maxAlpha);
        }

        public void OnGUI()
        {
            ZatsRenderer.DrawCross(new Vector2(Screen.width / 2f, Screen.height / 2f), Vector2.one * 16f, 1f, Color.white);
            if (Player.localPlayer != null)
            {
                DrawCrosshairs();
                DrawPlayers();
                if (PickupManager.instance != null)
                    DrawPickups();
                DrawRadar();
            }
        }
    }
}
