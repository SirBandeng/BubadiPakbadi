using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using HorrorSchool.Core;
using HorrorSchool.Inventory;
using HorrorSchool.Player;

namespace HorrorSchool.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform itemListContainer;
        [SerializeField] private Button itemButtonTemplate;

        [Header("Detail Item / Baca Catatan")]
        [SerializeField] private TextMeshProUGUI detailTitleText;
        [SerializeField] private TextMeshProUGUI detailContentText;

        private readonly List<GameObject> spawnedButtons = new List<GameObject>();
        private bool isOpen;
        private PlayerController cachedPlayer;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            Instance = this;
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            if (itemButtonTemplate != null) itemButtonTemplate.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // Jangan buka inventory saat dialog sedang aktif
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

            if (Keyboard.current.tabKey.wasPressedThisFrame || Keyboard.current.iKey.wasPressedThisFrame)
            {
                ToggleInventory();
            }
            else if (isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseInventory();
            }
        }

        public void ToggleInventory()
        {
            if (isOpen) CloseInventory();
            else OpenInventory();
        }

        public void OpenInventory()
        {
            isOpen = true;
            if (inventoryPanel != null) inventoryPanel.SetActive(true);
            SetPlayerControl(false);
            RefreshItemList();
        }

        public void CloseInventory()
        {
            isOpen = false;
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            SetPlayerControl(true);
        }

        private void RefreshItemList()
        {
            foreach (var btn in spawnedButtons)
            {
                Destroy(btn);
            }
            spawnedButtons.Clear();

            if (detailTitleText != null) detailTitleText.text = "Pilih Item / Catatan";
            if (detailContentText != null) detailContentText.text = "Klik item di sebelah kiri untuk memeriksa isinya.";

            if (GameStateManager.Instance == null || itemButtonTemplate == null) return;

            var items = GameStateManager.Instance.InventoryItems;
            for (int i = 0; i < items.Count; i++)
            {
                ItemData item = items[i];
                Button newBtn = Instantiate(itemButtonTemplate, itemListContainer);
                newBtn.gameObject.SetActive(true);

                var label = newBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = item.ItemName;

                newBtn.onClick.AddListener(() => ShowItemDetail(item));
                spawnedButtons.Add(newBtn.gameObject);
            }
        }

        private void ShowItemDetail(ItemData item)
        {
            if (detailTitleText != null) detailTitleText.text = item.ItemName;

            if (detailContentText != null)
            {
                if (item.ContentPages != null && item.ContentPages.Length > 0)
                {
                    detailContentText.text = string.Join("\n\n", item.ContentPages);
                }
                else
                {
                    detailContentText.text = "Item kunci untuk membuka pintu.";
                }
            }
        }

        private void SetPlayerControl(bool enabled)
        {
            if (cachedPlayer == null) cachedPlayer = FindFirstObjectByType<PlayerController>();
            if (cachedPlayer != null) cachedPlayer.SetMovementEnabled(enabled);
        }
    }
}