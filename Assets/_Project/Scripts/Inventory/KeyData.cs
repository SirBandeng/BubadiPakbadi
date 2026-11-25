using UnityEngine;

namespace HorrorSchool.Inventory
{
    [CreateAssetMenu(fileName = "NewKey", menuName = "HorrorSchool/Key Data")]
    public class KeyData : ItemData
    {
        [Header("Key Configurations")]
        [SerializeField] private string lockIdentifier; // Kunci ini membuka pintu apa?
        public string LockIdentifier => lockIdentifier;
    }
}