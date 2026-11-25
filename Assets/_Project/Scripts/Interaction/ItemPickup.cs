using UnityEngine;
using HorrorSchool.Core;
using HorrorSchool.Inventory;
using HorrorSchool.UI;

namespace HorrorSchool.Interaction
{
    public class ItemPickup : InteractableBase
    {
        [Header("Item Pickup Settings")]
        [SerializeField] private string uniquePickupID; // ID unik objek di scene, misal: PICKUP_KEY_SCHOOL
        [SerializeField] private ItemData itemData;
        [TextArea(2, 4)]
        [SerializeField] private string[] pickupDialogue;

        private void Start()
        {
            // Jika sudah pernah diambil sebelum pindah scene, langsung hilangkan
            if (GameStateManager.Instance != null && GameStateManager.Instance.IsWorldObjectCollected(uniquePickupID))
            {
                gameObject.SetActive(false);
            }
        }

        protected override void OnInteract(Player.PlayerController player)
        {
            if (itemData == null)
            {
                Debug.LogWarning($"ItemData belum diisi pada {gameObject.name}");
                return;
            }

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AddItem(itemData, uniquePickupID);
            }

            // Tampilkan dialog saat item diambil
            if (DialogueManager.Instance != null)
            {
                if (pickupDialogue != null && pickupDialogue.Length > 0)
                {
                    DialogueManager.Instance.StartDialogue(pickupDialogue);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(new[] { $"Mendapatkan: {itemData.ItemName}." });
                }
            }

            SetInteractableState(false);
            gameObject.SetActive(false);
        }
    }
}