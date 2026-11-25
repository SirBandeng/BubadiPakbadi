using System;
using System.Collections.Generic;
using UnityEngine;
using HorrorSchool.Inventory;

namespace HorrorSchool.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        // Encapsulation: Data disimpan privat, diakses lewat method publik
        private readonly List<ItemData> inventoryItems = new List<ItemData>();
        private readonly HashSet<string> collectedWorldObjectIDs = new HashSet<string>();
        private readonly HashSet<string> unlockedDoorIDs = new HashSet<string>();
        private readonly HashSet<string> storyFlags = new HashSet<string>();

        public string NextSpawnPointID { get; set; }

        public event Action OnInventoryUpdated;
        public IReadOnlyList<ItemData> InventoryItems => inventoryItems;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // --- MANAJEMEN ITEM ---
        public void AddItem(ItemData item, string worldObjectID = "")
        {
            if (item == null) return;

            if (!HasItem(item.ItemID))
            {
                inventoryItems.Add(item);
                OnInventoryUpdated?.Invoke();
            }

            if (!string.IsNullOrEmpty(worldObjectID))
            {
                collectedWorldObjectIDs.Add(worldObjectID);
            }
        }

        public bool HasItem(string itemID)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].ItemID == itemID) return true;
            }
            return false;
        }

        public bool IsWorldObjectCollected(string worldObjectID)
        {
            return !string.IsNullOrEmpty(worldObjectID) && collectedWorldObjectIDs.Contains(worldObjectID);
        }

        // --- MANAJEMEN PINTU TERKUNCI ---
        public void RegisterUnlockedDoor(string doorID)
        {
            if (!string.IsNullOrEmpty(doorID))
                unlockedDoorIDs.Add(doorID);
        }

        public bool IsDoorUnlocked(string doorID)
        {
            return !string.IsNullOrEmpty(doorID) && unlockedDoorIDs.Contains(doorID);
        }

        // --- MANAJEMEN EVENT CERITA ---
        public void SetFlag(string flagID) => storyFlags.Add(flagID);
        public bool HasFlag(string flagID) => storyFlags.Contains(flagID);
    }
}