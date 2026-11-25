using System;
using UnityEngine;

namespace HorrorSchool.Interaction
{
    public class InspectableObject : InteractableBase
    {
        [Header("Inspection Content")]
        [TextArea(2, 5)]
        [SerializeField] private string[] monologueLines;

        // Event statis sementara sebelum DialogueManager dibuat di Tahap 3
        public static event Action<string[]> OnInspectRequested;

        protected override void OnInteract(Player.PlayerController player)
        {
            if (monologueLines == null || monologueLines.Length == 0) return;

            // Memicu UI dialog (sementara juga mencetak ke Console untuk testing)
            Debug.Log($"[Atop Monolog]: {string.Join(" | ", monologueLines)}");
            OnInspectRequested?.Invoke(monologueLines);
        }
    }
}