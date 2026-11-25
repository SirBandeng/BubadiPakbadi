using System;
using UnityEngine;
using HorrorSchool.Core;
using HorrorSchool.UI;

namespace HorrorSchool.Interaction
{
    public abstract class DoorBase : InteractableBase
    {
        [Header("Door Lock Settings")]
        [SerializeField] private string doorID;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKeyID;
        [SerializeField] private string lockedMessage = "Pintunya terkunci rapat.";
        [SerializeField] private string unlockedMessage = "Pintu berhasil dibuka dengan kunci.";

        public static event Action<string> OnDoorFeedback;
        public bool IsLocked => isLocked;

        private void Start()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.IsDoorUnlocked(doorID))
            {
                isLocked = false;
            }
        }

        protected override void OnInteract(Player.PlayerController player)
        {
            if (isLocked)
            {
                if (!string.IsNullOrEmpty(requiredKeyID) && player.HasItem(requiredKeyID))
                {
                    UnlockDoor();
                    UseDoor(player);
                }
                else
                {
                    string msg = string.IsNullOrWhiteSpace(lockedMessage)
                        ? "Pintu utama terkunci rapat."
                        : lockedMessage;

                    int listenerCount = OnDoorFeedback != null ? OnDoorFeedback.GetInvocationList().Length : 0;
                    Debug.Log($"[DEBUG Pintu] Pesan: '{msg}' | Pendengar Event: {listenerCount} | DialogueManager Ada: {DialogueManager.Instance != null}");

                    if (listenerCount > 0)
                    {
                        OnDoorFeedback.Invoke(msg);
                    }
                    else if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.ShowSingleLine(msg);
                    }
                }
                return;
            }

            UseDoor(player);
        }

        public void UnlockDoor()
        {
            isLocked = false;
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.RegisterUnlockedDoor(doorID);
            }
        }

        public void LockDoor() => isLocked = true;

        protected abstract void UseDoor(Player.PlayerController player);
    }
}