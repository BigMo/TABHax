using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TABHax
{
    public static class PickupHelper
    {
        private static FieldInfo itemField = typeof(LootDatabase).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
        private static Dictionary<string, ItemDataEntry> nameMap = new Dictionary<string, ItemDataEntry>();
        public static Dictionary<int, ItemDataEntry> Items { get { return (Dictionary<int, ItemDataEntry>)itemField.GetValue(LootDatabase.Instance); } }
        
        private static void FillNameMap()
        {
            Console.Write("Items: ");
            var items = Items;
            Console.WriteLine(items.Keys.Count);
            int idx = 0;
            foreach (var kv in items)
            {
                if (kv.Value.prefab)
                {
                    Console.WriteLine("{0} - {1}", idx++, kv.Value.prefab.name);
                    nameMap[kv.Value.prefab.name] = kv.Value;
                }
            }
        }

        public static void Reset()
        {
            nameMap.Clear();
        }

        public static string[] GetAllItemNames()
        {
            if (nameMap.Count == 0)
                FillNameMap();

            return nameMap.Keys.ToArray();
        }
        public static int GetItemIndexByName(string name)
        {
            if (nameMap.Count == 0)
                FillNameMap();

            foreach (var kv in nameMap)
            {
                if (kv.Key.Trim() == name.Trim())
                    return kv.Value.pickup.m_itemIndex;
            }
            return -1;
        }

        public static bool GetItemByName(string name, out ItemDataEntry entry)
        {
            if (nameMap.Count == 0)
                FillNameMap();

            entry = new ItemDataEntry();
            foreach (var kv in nameMap)
            {
                if (kv.Key.Trim() == name.Trim())
                {
                    entry = nameMap[kv.Key];
                    return true;
                }
            }

            return false;
        }

        public static Pickup SpawnItem(int id)
        {
            Pickup spawnedItem = GameObject.Instantiate<Pickup>(LootDatabase.Instance.HMEFDENEPFC(id).pickup, Player.localPlayer.transform.position, Quaternion.identity);
            spawnedItem.canInteract = true;
            if (spawnedItem.weaponType == Pickup.JGHOAEDPDBB.Ammo || spawnedItem.weaponType == Pickup.JGHOAEDPDBB.Health)
                spawnedItem.m_quanitity = 999;
            else
                spawnedItem.m_quanitity = 1;
            Player.localPlayer.m_interactionHandler.PickUp(spawnedItem, true, Pickup.ODBCKHOEDEF.None, -1);
            return spawnedItem;
        }
    }
}
