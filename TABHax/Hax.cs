using Harmony;
using Landfall.Network;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TABHax.Dumping;
using TABHax.Game;
using TABHax.Patches;
using TABHax.Utils;
using UnityEngine;

namespace TABHax
{
    public class Hax : MonoBehaviour
    {
        private const UInt32 StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        static extern bool AllocConsole(int pid);
        [DllImport("msvcrt.dll")]
        public static extern int system(string cmd);

        public RingPillar[] pillars = new RingPillar[0];
        public PhotonServerHandler server;
        public bool TazeAllLookingAtMe = false;
        public bool TazeAllAroundMe = false;
        public float TazeDistance = 10f;
        public float floatForce = 10f;
        bool autoPickup;
        float autoPickupInterval = 0.5f;
        float autoPickupLast = 0;

        GameObject marker = null;

        public Hax()
        {
            CodeStage.AntiCheat.Detectors.InjectionDetector.Dispose();
            CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector.Dispose();
            CodeStage.AntiCheat.Detectors.SpeedHackDetector.Dispose();
            CodeStage.AntiCheat.Detectors.TimeCheatingDetector.Dispose();
            CodeStage.AntiCheat.Detectors.WallHackDetector.Dispose();

            AllocConsole(-1);
            var stdout = Console.OpenStandardOutput();
            var sw = new System.IO.StreamWriter(stdout, Encoding.Default);
            sw.AutoFlush = true;
            Console.SetOut(sw);
            Console.SetError(sw);

            Patch();
            StartCoroutine(Loop());
        }

        public void Start()
        {
            marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var rb = marker.AddComponent<Rigidbody>();
            marker.GetComponent<Renderer>().material.color = Color.red;
            marker.transform.localScale = Vector3.one * 0.2f;

            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            UnityEngine.Object.DontDestroyOnLoad(marker);
        }

        private void Patch()
        {
            Console.WriteLine("Installing patches...");
            try
            {
                var h = HarmonyInstance.Create("asdasdasd");
                h.Patch(typeof(Gun).GetMethod("Shoot"),
                    new HarmonyMethod(typeof(ShootPatch),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Gun patch!");

                h.Patch(typeof(MeleeWeapon).GetMethod("Attack"),
                    new HarmonyMethod(typeof(AttackPatch),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Melee patch!");

                h.Patch(typeof(AddScreenShake).GetMethod("DoShake"),
                    new HarmonyMethod(typeof(NoShake),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Shake patch!");

                h.Patch(typeof(WobbleShake).GetMethod("AddShake"),
                    new HarmonyMethod(typeof(NoShake2),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Shake patch2!");

                h.Patch(typeof(PositionShake).GetMethod("AddShakeWorld"),
                    new HarmonyMethod(typeof(NoShake3),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Shake patch3!");

                h.Patch(typeof(Recoil).GetMethod("AddRecoil"),
                    new HarmonyMethod(typeof(NoRecoil),
                    "Prefix"),
                    new HarmonyMethod());
                Console.WriteLine(" > Recoil patch!");

                Console.WriteLine("-> DONE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("... DONE?");
        }

        private bool CanHit(Player from, Player to)
        {
            var dir = to.m_torso.transform.position - from.m_head.transform.position;
            var cast = default(RaycastHit);
            if (Physics.Raycast(from.m_head.transform.position, dir.normalized, out cast, float.PositiveInfinity))
                return true;
            return false;
        }

        private Player GetNextTarget(Player lp, Player[] players)
        {
            Array.Sort(players, (a, b) =>
            {
                return (lp.m_head.transform.position - a.m_head.transform.position).sqrMagnitude
- (lp.m_head.transform.position - b.m_head.transform.position).sqrMagnitude
< 0 ? 1 : -1;
            });
            foreach (var p in players)
            {
                if (p == lp)
                    continue;
                Console.Write("A");
                if (CanHit(lp, p))
                    return p;
            }

            return null;
        }

        private void TeleportToCrosshair()
        {
            RaycastHit hit;
            if (Physics.Raycast(Player.localPlayer.m_playerCamera.transform.position + Player.localPlayer.m_playerCamera.transform.forward.normalized * 2f, Player.localPlayer.m_playerCamera.transform.forward.normalized, out hit))
                MovePlayer(hit.point + Vector3.up * 3f);
        }

        public void MovePlayer(Vector3 position)
        {
            Rigidbody[] allRigs = Player.localPlayer.GetComponent<RigidbodyHolder>().AEGMKLJJLLA().ToArray();
            for (int i = 0; i < allRigs.Length; i++)
                allRigs[i].gameObject.transform.position = position;

            if (server)
            {
                var transform = Player.localPlayer.m_playerCamera.gameObject.transform;
                Vector2 rotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
                server.SendPlayerUpdate(transform.position, rotation, Player.localPlayer.m_playerCamera.gameObject.transform.forward, 1);
            }
        }

        private void Skydive()
        {
            Skydive(Player.localPlayer.m_playerCamera.transform.forward);
        }
        private void Skydive(Vector3 direction)
        {
            Skydiving dive = Player.localPlayer.gameObject.GetComponent<Skydiving>();
            if (dive == null)
                dive = Player.localPlayer.gameObject.AddComponent<Skydiving>();
            dive.Launch(direction * 10f);
        }

        private void LaunchOff(Player player)
        {
            if (!player)
                return;

            Skydiving dive = player.gameObject.GetComponent<Skydiving>();
            if (dive == null)
                dive = player.gameObject.AddComponent<Skydiving>();
            dive.Launch(Vector3.up);
        }

        /// <summary>
        /// <para>Will pickup any item disregarding range by telling the server we're at the item's position.</para>
        /// <para>Process: <c>SendPlayerUpdate(pickupPosition) => Pickup() => SendPlayerUpdate(oldPosition)</c></para>
        /// </summary>
        /// <remarks>Buggy.</remarks>
        /// <param name="pickup">The Pickup instance of the item we're getting</param>
        public void ForcePickup(Pickup pickup)
        {
            var oldPos = Player.localPlayer.m_torso.transform;
            var fakeNewPos = pickup.gameObject.transform;
            if (server)
            {
                var transform = Player.localPlayer.m_playerCamera.gameObject.transform;
                Vector2 rotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
                server.SendPlayerUpdate(fakeNewPos.position, rotation, fakeNewPos.forward, 1);
            }
            Player.localPlayer.m_interactionHandler.PickUp(pickup, false, pickup.equipSlots, 1);
            if (server)
            {
                var transform = Player.localPlayer.m_playerCamera.gameObject.transform;
                Vector2 rotation = new Vector2(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
                server.SendPlayerUpdate(oldPos.position, rotation, oldPos.forward, 1);
            }
        }

        private static void DumpItems()
        {
            for (int i = 0; i < LootDatabase.Instance.ItemCount; i++)
            {
                var spawnedItem = GameObject.Instantiate<Pickup>(LootDatabase.Instance.HMEFDENEPFC(i).pickup, Player.localPlayer.transform.position, Quaternion.identity);
                Console.WriteLine("{0} | {1} | {2}", i, spawnedItem.name, spawnedItem.weaponType);
            }
        }

        private void PerformTaze()
        {
            if (server && TazeAllAroundMe)
                foreach (var p in PlayerManager.AlivePlayers)
                    if (p && (p.GetHeadTransform().position - PlayerManager.LocalPlayer.GetHeadTransform().position).magnitude <= TazeDistance)
                        TazePlayer(p, 1f);

            if (server && TazeAllLookingAtMe)
                foreach (var p in PlayerManager.LookingAtMe)
                    TazePlayer(p, 1f);
        }

        private bool wasNull = true;
        private IEnumerator Loop()
        {
            while (this.enabled && this.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(1);
                try
                {
                    if (Player.localPlayer == null)
                    {
                        wasNull = true;
                        Console.WriteLine("~Local player null~");
                        continue;
                    }

                    EncolorObject(Player.localPlayer.transform, ShaderManager.TransparentMat005);

                    Console.Write("Pos: {0}", Player.localPlayer.m_playerCamera.transform.position);

                    PlayerManager.Update();

                    pillars = FindObjectsOfType<RingPillar>();

                    if (!server)
                        server = FindObjectOfType<PhotonServerHandler>();

                    if (PlayerManager.AllPlayers.Length == 0)
                    {
                        Console.WriteLine(" | No players");
                        continue;
                    }
                    Console.Write(" | {0} players", PlayerManager.AllPlayers.Length);
                    foreach (var e in PlayerManager.AlivePlayers)
                        EncolorObject(e.transform, ShaderManager.MatRed);

                    if (PickupManager.instance == null || PickupManager.instance.m_Pickups == null || PickupManager.instance.m_Pickups.Count == 0)
                    {
                        Console.WriteLine(" | No pickups");
                        continue;
                    }

                    Console.Write(" | {0} pickups", PickupManager.instance.m_Pickups.Count);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void EncolorObject(Transform t, Material mat, bool includeChildren = true)
        {
            if (includeChildren)
            {
                var children = t.root.GetComponentsInChildren<Transform>();
                foreach (var _t in children)
                    ChangeRendererMaterial(_t, mat);
            }
            else
            {
                ChangeRendererMaterial(t, mat);
            }
        }
        private void ChangeRendererMaterial(Transform t, Material mat)
        {
            if (t.GetComponent<TABGWeapon>())
                return;

            var r = t.GetComponent<Renderer>();
            if (r)
            {

                for (int i = 0; i < r.materials.Length; i++)
                    r.materials[i] = mat;
                r.material = mat;
            }
        }

        private void Heal()
        {
            float pHealth = Player.localPlayer.m_playerDeath.health;
            if (pHealth < 100)
                server?.ClientRequestHeal(PickupHelper.GetItemIndexByName("Med Kit"));
        }

        public void FixedUpdate()
        {
            Heal();

            if (Input.GetKey(KeyCode.Mouse2))
            {
                var nearest = GunManager.NearestFov.m_head.transform;
                GunManager.AimAt(GunManager.LeftGun, nearest);
                GunManager.AimAt(GunManager.RightGun, nearest);
            }

            var allRigs = Player.localPlayer.GetComponent<RigidbodyHolder>().ECGEEPIPDHL().ToArray();
            var colliders = Player.localPlayer.GetComponentsInChildren<Collider>();
            foreach (var r in allRigs)
            {
                r.useGravity = !fly;
                r.collisionDetectionMode = fly ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
            }
            foreach (var c in colliders)
                c.enabled = !fly;
        }

        public bool fly;

        public void Update()
        {
            marker.transform.position = PlayerManager.LocalPlayer.m_head.transform.position;
            if (Input.GetKeyDown(KeyCode.F8))
                DumpItems();

            if (Input.GetKey(KeyCode.End))
                Destroy(this);

            if (Player.localPlayer == null)
                return;

            if (Input.GetKey(KeyCode.K) && PlayerManager.AllPlayers.Length > 0)
                foreach (var p in PlayerManager.AlivePlayers)
                    if (p != Player.localPlayer)
                        p.m_playerDeath.TakeDamage(Vector3.one * 30f, Vector3.one, false, null, false, false, null, null);

            PerformTaze();

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                fly = !fly;

            if (Input.GetKeyDown(KeyCode.Q))
                Skydive();

            if (Input.GetKeyDown(KeyCode.L))
            {
                foreach (var i in PickupHelper.GetAllItemNames())
                {
                    ItemDataEntry e;
                    if (PickupHelper.GetItemByName(i, out e))
                    {
                        Console.WriteLine("DROPPING {0}", e.pickup.name);
                        Player.localPlayer.m_interactionHandler.Drop(e.pickup.m_itemIndex, 10, true, e.pickup.equipSlots, false, false);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.F1))
                server?.TranscendToGod();

            if (Input.GetKeyDown(KeyCode.F2))
                server?.RipShitUp();

            if (Input.GetKeyDown(KeyCode.T))
                TeleportToCrosshair(); ;

            if (Input.GetKey(KeyCode.B))
                TazePlayer(PlayerManager.Nearest, 5f);

            if (Input.GetKey(KeyCode.N))
                TazePlayer(PlayerManager.Target, 5f);

            if (Input.GetKey(KeyCode.J) && server)
            {
                var t = PlayerManager.Target;
                if (t)
                    server.ClientDoEffect(t.GetID(), MKABDBMIHKD.Skydiving, 5f);
            }

            if (Input.GetKey(KeyCode.Mouse3) && Player.localPlayer)
                Player.localPlayer.GetComponent<InputHandler>().inputMovementDirection += Player.localPlayer.m_playerCamera.transform.forward * 20f;

            if (Input.GetKey(KeyCode.Mouse4))
            {
                marker.transform.position = Player.localPlayer.m_playerCamera.transform.position
                    + Vector3.up * UnityEngine.Random.Range(0f, 2f)
                    + Vector3.left * UnityEngine.Random.Range(-5f, 5f)
                    + Vector3.forward * UnityEngine.Random.Range(-5f, 5f);

                server?.SendPlayerUpdate(marker.transform.position,
                    new Vector2(UnityEngine.Random.Range(-179, 179), UnityEngine.Random.Range(-179, 179)),
                    new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)),
                    5);
            }

            PlayerManager.LocalPlayer.m_weaponHandler?.leftWeapon?.GetComponentInChildren<Railing>(true)?.gameObject.SetActive(true);
            PlayerManager.LocalPlayer.m_weaponHandler?.rightWeapon?.GetComponentInChildren<Railing>(true)?.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                DumpLayers();

            if (Input.GetKey(KeyCode.U))
                foreach (var p in PlayerManager.LookingAtMe)
                    server.ClientDoEffect(p.GetID(), MKABDBMIHKD.Tase, 5f);
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                autoPickup = !autoPickup;

            if (autoPickup && PickupManager.instance)// && autoPickupLast + autoPickupInterval < Time.time)
            {
                var p = GetNearestPickup(PickupManager.instance.m_Pickups);
                if (p)
                {
                    Console.WriteLine("Picking up {0}", p.name);
                    var oldPos = PlayerManager.LocalPlayer.m_torso.transform;
                    var rotation = new Vector2(PlayerManager.LocalPlayer.m_torso.transform.rotation.eulerAngles.x, PlayerManager.LocalPlayer.m_torso.transform.rotation.eulerAngles.y);

                    server?.SendPlayerUpdate(p.transform.position, rotation, p.transform.forward, 1);
                    p.canInteract = true;
                    Player.localPlayer.m_interactionHandler.PickUp(p, false, p.equipSlots, 1);

                    server?.SendPlayerUpdate(oldPos.position, rotation, oldPos.forward, 1);
                    autoPickupLast = Time.time;
                }
            }
        }

        Pickup lastPickup;
        private Pickup GetNearestPickup(IEnumerable<Pickup> list)
        {
            float minDist = float.MaxValue;
            Pickup p = null;
            foreach (var pu in list)
            {
                if (!pu || lastPickup == pu || !pu.canInteract)
                    continue;

                if (pu.weaponType == Pickup.JGHOAEDPDBB.Weapon)// || pu.weaponType == Pickup.JGHOAEDPDBB.Ammo)
                    continue;

                if (pu.equipSlots == Pickup.ODBCKHOEDEF.ArmorSlot && PlayerManager.LocalPlayer.m_inventory.MPBCIGPFDGO(Pickup.ODBCKHOEDEF.ArmorSlot))
                    continue;
                if (pu.equipSlots == Pickup.ODBCKHOEDEF.HeadSlot && PlayerManager.LocalPlayer.m_inventory.MPBCIGPFDGO(Pickup.ODBCKHOEDEF.HeadSlot))
                    continue;

                //if (pu.PHDMFNFACCC && ((TABGWeapon)pu.PHDMFNFACCC).GetField<Player>("GCFKMFGMBNO") == Player.localPlayer)
                //    continue;

                var dist = (pu.transform.position - PlayerManager.LocalPlayer.m_torso.transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    p = pu;
                }
            }
            return lastPickup = p;
        }

        private void TazePlayer(Player player, float duration)
        {
            if (player && server && player != PlayerManager.LocalPlayer)
                server.ClientDoEffect(player.GetID(), MKABDBMIHKD.Tase, duration);
        }

        private void DumpLayers()
        {
            var unknownId = 0;
            for (var i = 0; i < 32; i++)
            {
                var layer = LayerMask.LayerToName(i);
                Console.WriteLine("{1} = 1 << {0},", i,
                    !string.IsNullOrEmpty(layer) ? layer : string.Format("__unknown{0}", unknownId++));
            }
        }

        private static void Test()
        {
            string t;
            InTest(t);
        }

        private static void InTest(in string test)
        {
            Console.WriteLine("Argument: {0}", test);
        }
    }
}
