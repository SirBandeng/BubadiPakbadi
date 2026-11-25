using UnityEngine;

namespace HorrorSchool.Inventory
{
    public enum ItemCategory
    {
        Key,
        DocumentOrNote,
        Photo
    }

    [CreateAssetMenu(fileName = "NewItem", menuName = "HorrorSchool/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identitas Item")]
        [SerializeField] private string itemID; // Contoh: KEY_SCHOOL, NOTE_PAGE_1
        [SerializeField] private string itemName;
        [SerializeField] private ItemCategory category;
        [SerializeField] private Sprite icon;

        [Header("Isi Bacaan / Deskripsi (Untuk Catatan & Foto)")]
        [TextArea(3, 8)]
        [SerializeField] private string[] contentPages;

        public string ItemID => itemID;
        public string ItemName => itemName;
        public ItemCategory Category => category;
        public Sprite Icon => icon;
        public string[] ContentPages => contentPages;
    }
}