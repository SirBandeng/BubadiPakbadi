using UnityEngine;

namespace HorrorSchool.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Base Interaction Settings")]
        [SerializeField] private string promptText = "[E] Periksa";
        [SerializeField] private GameObject promptIndicatorUI; // Opsional: ikon tanda seru / tombol E di atas objek
        [SerializeField] private bool isInteractable = true;

        // Encapsulation: Akses read-only dari luar kelas
        public string PromptText => promptText;
        public virtual bool CanInteract => isInteractable;

        protected virtual void Awake()
        {
            // Pastikan collider bertindak sebagai trigger deteksi interaksi
            GetComponent<Collider2D>().isTrigger = true;
            ShowPrompt(false);
        }

        public void ShowPrompt(bool isVisible)
        {
            if (promptIndicatorUI != null)
            {
                promptIndicatorUI.SetActive(isVisible && CanInteract);
            }
        }

        public void Interact(Player.PlayerController player)
        {
            if (!CanInteract) return;
            OnInteract(player);
        }

        // Polymorphism: Wajib di-override oleh setiap kelas turunan
        protected abstract void OnInteract(Player.PlayerController player);

        protected void SetInteractableState(bool state)
        {
            isInteractable = state;
            if (!state) ShowPrompt(false);
        }
    }
}