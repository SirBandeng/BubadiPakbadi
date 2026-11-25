using UnityEngine;
using HorrorSchool.Core;
using HorrorSchool.Player;

namespace HorrorSchool.Horror
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class StoryTriggerBase : MonoBehaviour
    {
        [Header("Base Story Trigger")]
        [SerializeField] private string triggerID; // ID unik agar event tidak terulang jika sudah terjadi
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggeredLocally;

        protected virtual void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void Start()
        {
            if (triggerOnce && GameStateManager.Instance != null && GameStateManager.Instance.HasFlag(triggerID))
            {
                hasTriggeredLocally = true;
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerOnce && hasTriggeredLocally) return;

            if (other.TryGetComponent<PlayerController>(out var player))
            {
                hasTriggeredLocally = true;
                if (triggerOnce && GameStateManager.Instance != null && !string.IsNullOrEmpty(triggerID))
                {
                    GameStateManager.Instance.SetFlag(triggerID);
                }

                OnTriggered(player);
            }
        }

        // Polymorphism: Setiap kejadian cerita/horor punya aksi berbeda
        protected abstract void OnTriggered(PlayerController player);
    }
}