using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TABHax.Game;
using TABHax.Patches;
using TABHax.UI;
using TABHax.UI.Controls;
using UnityEngine;

namespace TABHax.UI
{
    public class TABUI : MonoBehaviour
    {
        private int mainWindowID = 1000,
            itemWindowID = 1001,
            espWindowID = 1002,
            playerWindowID = 1003,
            miniMapWindowID = 1004;


        private static Vector2 margin = new Vector2(4, 4);
        private static Vector2 titleSize = new Vector2(0, 18);

        private static float mapToScreen = 0.2f,
            worldToMap = 0.25f;
        private static Vector2 miniMapOriginalSize = new Vector2(1024, 1024),
            miniMapSize = miniMapOriginalSize * mapToScreen,
            miniMapMarkerSize = new Vector2(64, 64);

        private static Vector2 mainWindowSize = new Vector2(200, 140),
            itemWindowSize = new Vector2(300, 400),
            espWindowSize = new Vector2(200, 400),
            playerWindowSize = new Vector2(300, 400),
            miniMapWindowSize = titleSize + miniMapSize + margin * 2f;

        private Rect mainWindowRect = new Rect(Vector2.zero, mainWindowSize),
            itemWindowRect = new Rect(mainWindowSize + margin, itemWindowSize),
            espWindowRect = new Rect(mainWindowSize + margin, espWindowSize),
            playerWindowRect = new Rect(mainWindowSize + margin, playerWindowSize),
            miniMapWindowRect = new Rect(mainWindowSize + margin, miniMapWindowSize);

        private Vector2 itemWindowScroll = Vector2.zero,
            playerWindowScroll = Vector2.zero;

        private bool showItems = false,
            showEsp = false,
            showPlayer = false,
            showMiniMap = false;
        private Texture2D miniMapTexture;

        private bool itemShowWeapons = true,
            itemShowAttachments = true,
            itemShowArmor = true,
            itemShowOthers = true;

        private Window patchWindow;

        public void Start()
        {
            miniMapTexture = new Texture2D(2, 2);
            var finalMap = Properties.Resources.FinalFinalMap_Clipped;

            ImageConversion.LoadImage(miniMapTexture, finalMap);

            patchWindow = new Window();
            patchWindow.Text = "Patches";
            patchWindow.Size = new Vector2(300, 300);
            patchWindow.Position = mainWindowRect.position + mainWindowRect.size;
            patchWindow.DragSize = patchWindow.Size;

            var layout = new LinearLayout();
            layout.SizeMode = Control.eSizeMode.Manual;
            layout.Position = new Vector2(4, 20);
            layout.Size = patchWindow.Size - layout.Position;
            layout.AddChild(CreatePatchButton("NoShake", NoShake.Active, () => NoShake.Active, (v) => NoShake.Active = NoShake2.Active = NoShake3.Active = v));
            layout.AddChild(CreatePatchButton("NoStun", NoStun.Active, () => NoStun.Active, (v) => NoStun.Active = v));
            layout.AddChild(CreatePatchButton("NoRecoil", NoRecoil.Active, () => NoRecoil.Active, (v) => NoRecoil.Active = v));
            layout.AddChild(CreatePatchButton("NoSpread", ShootPatch.NoSpread, () => ShootPatch.NoSpread, (v) => ShootPatch.NoSpread = v));
            layout.AddChild(CreatePatchButton("InfiniteAmmo", ShootPatch.InfiniteAmmo, () => ShootPatch.InfiniteAmmo, (v) => ShootPatch.InfiniteAmmo = v));
            layout.AddChild(CreatePatchButton("InstantFire", ShootPatch.InstantFire, () => ShootPatch.InstantFire, (v) => ShootPatch.InstantFire = v));
            layout.AddChild(CreatePatchButton("FullAuto", ShootPatch.AutoFire, () => ShootPatch.AutoFire, (v) => ShootPatch.AutoFire = v));
            layout.AddChild(CreatePatchButton("InstantMelee", AttackPatch.Active, () => AttackPatch.Active, (v) => AttackPatch.Active = v));
            layout.AddChild(CreatePatchButton("TazeArea", false, () => Loader.HaxInstance.TazeAllAroundMe, (v) => Loader.HaxInstance.TazeAllAroundMe = v));
            layout.AddChild(CreatePatchButton("TazeLooking", false, () => Loader.HaxInstance.TazeAllLookingAtMe, (v) => Loader.HaxInstance.TazeAllLookingAtMe = v));
            patchWindow.AddChild(layout);
            layout.ReorderChildren();
        }

        private Button CreatePatchButton(string text, bool initialState, Func<bool> getValue, Action<bool> setValue)
        {
            var btn = new Button();
            btn.Text = string.Format("{0}: {1}", text, initialState ? "ON" : "OFF");
            btn.SizeMode = Control.eSizeMode.AutoSize;
            btn.Click += (s, e) =>
            {
                var currentValue = !getValue();
                setValue(currentValue);
                var b = (Button)s;
                btn.Text = string.Format("{0}: {1}", text, currentValue ? "ON" : "OFF");
            };
            return btn;
        }

        public void OnGUI()
        {
            mainWindowRect = GUI.Window(mainWindowID, mainWindowRect, DoMainWindow, "TABHax");
            if (showItems)
            {
                ZatsRenderer.DrawLine(mainWindowRect.position, itemWindowRect.position, 1f, Color.white);
                itemWindowRect = GUI.Window(itemWindowID, itemWindowRect, DoItemWindow, "Items");
            }
            if (showEsp)
            {
                ZatsRenderer.DrawLine(mainWindowRect.position, espWindowRect.position, 1f, Color.white);
                espWindowRect = GUI.Window(espWindowID, espWindowRect, DoEspWindow, "ESP Settings");
            }
            if (showPlayer)
            {
                ZatsRenderer.DrawLine(mainWindowRect.position, playerWindowRect.position, 1f, Color.white);
                playerWindowRect = GUI.Window(playerWindowID, playerWindowRect, DoPlayerWindow, "Players");
            }

            if (showMiniMap)
            {
                ZatsRenderer.DrawLine(mainWindowRect.position, miniMapWindowRect.position, 1f, Color.white);
                miniMapWindowRect = GUI.Window(miniMapWindowID, miniMapWindowRect, DoMiniMapWindow, "Map");
            }

            patchWindow.Draw();

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        }

        private Vector2 WorldToMap(Vector3 pos)
        {
            return miniMapSize * 0.5f + new Vector2(pos.x, pos.z * -1f) * worldToMap * mapToScreen;
        }
        private void DrawPlayer(Vector2 offset, Player player, Color color)
        {
            var p = WorldToMap(player.m_torso.transform.position);
            ZatsRenderer.DrawDot(offset + p, color);
        }
        private void DoMiniMapWindow(int id)
        {
            var pos = new Vector2(4, 18);

            if (miniMapTexture)
            {
                GUI.DrawTexture(new Rect(pos, miniMapSize), miniMapTexture);

                if (TheRing.Instance && TheRing.Instance.currentRingID >= 0 && Loader.HaxInstance.pillars != null && Loader.HaxInstance.pillars.Length > 0)
                {
                    //foreach (var p in Loader.HaxInstance.pillars)
                    //    ZatsRenderer.DrawDot(pos + WorldToMap(p.pillar.transform.position), Color.yellow);
                    for(int i = 0; i <Loader.HaxInstance.pillars.Length-1; i++)
                    {
                        ZatsRenderer.DrawLine(
                            pos + WorldToMap(Loader.HaxInstance.pillars[i].pillar.transform.position),
                            pos + WorldToMap(Loader.HaxInstance.pillars[i+1].pillar.transform.position),
                            1f, Color.yellow);
                    }
                    ZatsRenderer.DrawLine(
                        pos + WorldToMap(Loader.HaxInstance.pillars[0].pillar.transform.position),
                        pos + WorldToMap(Loader.HaxInstance.pillars[Loader.HaxInstance.pillars.Length - 1].pillar.transform.position),
                        1f, Color.yellow);

                    var progress = 1f - (TheRing.Instance.timeToTravel - TheRing.Instance.timeTravelled) / TheRing.Instance.timeToTravel;
                    var timeLeft = TheRing.Instance.timeToTravel - TheRing.Instance.timeTravelled;
                    if (progress > 0f && progress < 1f)
                        ZatsRenderer.DrawString(pos, string.Format("Ring: {0:P2} ({1:0.0}s)", progress, timeLeft), Color.white, false);
                }

                if (PlayerManager.AlivePlayers.Length > 0)
                    foreach (var p in PlayerManager.AlivePlayers)
                        DrawPlayer(pos, p, Color.red);

                if (Player.localPlayer)
                {
                    DrawPlayer(pos, Player.localPlayer, Color.white);
                    var p1 = pos + WorldToMap(Player.localPlayer.m_torso.transform.position);
                    var p2 = pos + WorldToMap(Player.localPlayer.m_playerCamera.transform.position + Player.localPlayer.m_playerCamera.transform.forward * 250f);
                    ZatsRenderer.DrawLine(p1, p2, 1f, Color.white);
                }
            }

            GUI.DragWindow(new Rect(0, 0, 1000, 18));
        }

        private void DoPlayerWindow(int id)
        {
            var pos = new Vector2(4, 18);
            var buttonKill = GUI.skin.button.CalcSize(new GUIContent("Kill"));
            var buttonTeleport = GUI.skin.button.CalcSize(new GUIContent("Teleport"));

            var players = PlayerManager.AlivePlayers;
            var viewSize = new Vector2(playerWindowSize.x - margin.x * 2 - pos.x + GUI.skin.verticalScrollbar.fixedWidth, players.Length * buttonKill.y);

            playerWindowScroll = GUI.BeginScrollView(new Rect(pos, playerWindowSize - pos - margin), playerWindowScroll, new Rect(pos, viewSize));
            var labelWidth = viewSize.x - buttonKill.x - buttonTeleport.x - GUI.skin.verticalScrollbar.fixedWidth;

            Array.Sort(players, (a, b) => a.name.CompareTo(b.name));
            var bPos = pos;
            foreach (var p in players)
            {
                GUI.Label(new Rect(bPos.x, bPos.y, labelWidth, buttonKill.y), p.name);
                if (GUI.Button(new Rect(bPos.x + labelWidth - GUI.skin.verticalScrollbar.fixedWidth, bPos.y, buttonTeleport.x, buttonTeleport.y), "Teleport"))
                {
                    Loader.HaxInstance.MovePlayer(p.m_head.transform.position);
                }
                if (GUI.Button(new Rect(bPos.x + labelWidth - GUI.skin.verticalScrollbar.fixedWidth + buttonTeleport.x, bPos.y, buttonKill.x, buttonKill.y), "Kill"))
                {
                    p.m_playerDeath.TakeDamage(Vector3.one * 30f, Vector3.one, false, null, false, false, null, null);
                }
                bPos.y += buttonKill.y;
            }

            GUI.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 1000, 18));
        }

        private void DoEspWindow(int id)
        {
            var pos = new Vector2(4, 18);
            var buttonHeight = GUI.skin.button.CalcSize(new GUIContent("Toggle item window")).y;
            var sliderHeight = GUI.skin.horizontalSlider.lineHeight;
            var buttonSize = new Vector2(espWindowSize.x - margin.x * 2f, buttonHeight);

            var nextPos = pos + margin;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Players: {0}", Loader.ESPInstance.CfgDrawPlayers ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawPlayers = !Loader.ESPInstance.CfgDrawPlayers;
            nextPos += Vector2.up * buttonHeight;
            Loader.ESPInstance.CfgPlayerDistance = GUI.HorizontalSlider(new Rect(nextPos, buttonSize), Loader.ESPInstance.CfgPlayerDistance, 10f, 9999f);

            nextPos += Vector2.up * buttonHeight;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Weapons: {0}", Loader.ESPInstance.CfgDrawWeapons ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawWeapons = !Loader.ESPInstance.CfgDrawWeapons;
            nextPos += Vector2.up * buttonHeight;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Attachments: {0}", Loader.ESPInstance.CfgDrawAttachments ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawAttachments = !Loader.ESPInstance.CfgDrawAttachments;
            nextPos += Vector2.up * buttonHeight;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Armor: {0}", Loader.ESPInstance.CfgDrawArmor ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawArmor = !Loader.ESPInstance.CfgDrawArmor;
            nextPos += Vector2.up * buttonHeight;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Health: {0}", Loader.ESPInstance.CfgDrawHealth ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawHealth = !Loader.ESPInstance.CfgDrawHealth;
            nextPos += Vector2.up * buttonHeight;
            if (GUI.Button(new Rect(nextPos, buttonSize), string.Format("Draw Others: {0}", Loader.ESPInstance.CfgDrawOthers ? "ON" : "OFF")))
                Loader.ESPInstance.CfgDrawOthers = !Loader.ESPInstance.CfgDrawOthers;

            nextPos += Vector2.up * buttonHeight;
            Loader.ESPInstance.CfgPickupDistance = GUI.HorizontalSlider(new Rect(nextPos, buttonSize), Loader.ESPInstance.CfgPickupDistance, 10f, 9999f);

            nextPos += Vector2.up * buttonHeight;


            GUI.DragWindow(new Rect(0, 0, 1000, 18));
        }

        private void DoMainWindow(int id)
        {
            var pos = new Vector2(4, 18);

            var buttonSize = GUI.skin.button.CalcSize(new GUIContent("Toggle item window"));
            buttonSize.x = mainWindowSize.x - margin.x * 2;
            if (GUI.Button(new Rect(pos, buttonSize), "Toggle item window"))
                showItems = !showItems;

            pos += Vector2.up * buttonSize.y;
            if (GUI.Button(new Rect(pos, buttonSize), "Toggle esp window"))
                showEsp = !showEsp;

            pos += Vector2.up * buttonSize.y;
            if (GUI.Button(new Rect(pos, buttonSize), "Toggle player window"))
                showPlayer = !showPlayer;

            pos += Vector2.up * buttonSize.y;
            if (GUI.Button(new Rect(pos, buttonSize), "Toggle minimap window"))
                showMiniMap = !showMiniMap;

            pos += Vector2.up * buttonSize.y;
            GUI.Label(new Rect(pos, buttonSize), string.Format("FloatForce: {0}", Loader.HaxInstance.floatForce));

            pos += Vector2.up * buttonSize.y;
            Loader.HaxInstance.floatForce = GUI.HorizontalSlider(new Rect(pos, buttonSize), Loader.HaxInstance.floatForce, 0f, 500f);


            GUI.DragWindow(new Rect(0, 0, 1000, 18));
        }


        private ItemDataEntry[] FilterItems()
        {
            var entries = new List<ItemDataEntry>();
            var itemNames = PickupHelper.GetAllItemNames();
            for (int i = 0; i < itemNames.Length; i++)
            {
                ItemDataEntry item;
                if (PickupHelper.GetItemByName(itemNames[i], out item))
                {
                    switch (item.pickup.weaponType)
                    {
                        case Pickup.JGHOAEDPDBB.Weapon:
                            if (itemShowWeapons)
                                entries.Add(item);
                            break;
                        case Pickup.JGHOAEDPDBB.WeaponAttatchment:
                            if (itemShowAttachments)
                                entries.Add(item);
                            break;
                        case Pickup.JGHOAEDPDBB.Armor:
                            if (itemShowArmor)
                                entries.Add(item);
                            break;
                        default:
                            if (itemShowOthers)
                                entries.Add(item);
                            break;
                    }
                }
            }

            return entries.ToArray();
        }

        private void DoItemWindow(int windowId)
        {
            var items = FilterItems();
            var buttonSize = GUI.skin.button.CalcSize(new GUIContent("Item"));

            var pos = new Vector2(4, 18);
            var viewSize = new Vector2(itemWindowSize.x - margin.x - pos.x, items.Length * buttonSize.y);

            if (GUI.Button(new Rect(
                pos.x + margin.x,
                pos.y + margin.y,
                (itemWindowSize - pos).x / 2f, buttonSize.y),
                "Toggle Weapons"))
                itemShowWeapons = !itemShowWeapons;

            if (GUI.Button(new Rect(
                (itemWindowSize - pos).x / 2f + pos.x + margin.x,
                pos.y + margin.y,
                (itemWindowSize - pos).x / 2f, buttonSize.y),
                "Toggle Attachments"))
                itemShowAttachments = !itemShowAttachments;


            if (GUI.Button(new Rect(
                pos.x + margin.x,
                pos.y + margin.y + buttonSize.y + margin.y,
                (itemWindowSize - pos).x / 2f, buttonSize.y),
                "Toggle Armor"))
                itemShowArmor = !itemShowArmor;

            if (GUI.Button(new Rect(
                (itemWindowSize - pos).x / 2f + pos.x + margin.x,
                pos.y + margin.y + buttonSize.y + margin.y,
                (itemWindowSize - pos).x / 2f, buttonSize.y),
                "Toggle Others"))
                itemShowOthers = !itemShowOthers;

            var size = itemWindowSize - pos - margin - Vector2.up * (pos.y + margin.y + buttonSize.y + margin.y + buttonSize.y + margin.y);
            pos += Vector2.up * (buttonSize.y + margin.y + buttonSize.y + margin.y);

            //Scroll
            itemWindowScroll = GUI.BeginScrollView(new Rect(pos, size), itemWindowScroll, new Rect(pos, viewSize));

            Array.Sort(items, (a, b) => a.pickup.name.CompareTo(b.pickup.name));

            for (int i = 0; i < items.Length; i++)
            {
                var sprite = items[i].weaponIcon != null ? items[i].weaponIcon : items[i].icon;
                if (sprite != null)
                {
                    float widthFactor = 1f, heightFactor = 1f;
                    if (sprite.textureRect.width > sprite.textureRect.height)
                        heightFactor = sprite.textureRect.height / sprite.textureRect.width;
                    else if (sprite.textureRect.width < sprite.textureRect.height)
                        widthFactor = sprite.textureRect.width / sprite.textureRect.height;

                    var sprSize = new Vector2(buttonSize.y * widthFactor, buttonSize.y * heightFactor);
                    var sprPos = new Vector2(pos.x + margin.x, pos.y + i * buttonSize.y);
                    var slotSize = new Vector2(buttonSize.y, buttonSize.y);

                    GUI.DrawTexture(new Rect(
                            sprPos + slotSize / 2f - sprSize / 2f,
                            sprSize),
                            sprite.texture);
                }
                if (GUI.Button(new Rect(
                    pos.x + margin.x * 2f + buttonSize.y,
                    pos.y + i * buttonSize.y,
                    viewSize.x - pos.x + margin.x - margin.x * 2 - buttonSize.y - GUI.skin.verticalScrollbar.fixedWidth,
                    buttonSize.y),
                    string.Format("{0} [{1}]", items[i].pickup.name, items[i].pickup.weaponType)))
                    PickupHelper.SpawnItem(items[i].pickup.m_itemIndex);
            }
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 1000, 18));
        }
    }
}
