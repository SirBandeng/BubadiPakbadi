using System;
using UnityEngine;
using HorrorSchool.Interaction;
using HorrorSchool.Player;

namespace HorrorSchool.Puzzle
{
    public class InteractiveSymbolObject : InteractableBase
    {
        [Header("Symbol Configuration")]
        [SerializeField] private string symbolID; // Contoh: JAM, LILIN, BULAN, KUNCI
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite activeSprite;

        private SpriteRenderer spriteRenderer;
        private bool isActive = false;

        public static event Action<string, bool> OnSymbolStateChanged;
        public string SymbolID => symbolID;
        public bool IsActive => isActive;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateVisual();
        }

        protected override void OnInteract(PlayerController player)
        {
            // Toggle state (aktif / tidak aktif)
            isActive = !isActive;
            UpdateVisual();

            // Beritahu Puzzle Manager lewat event
            OnSymbolStateChanged?.Invoke(symbolID, isActive);
        }

        public void ForceDeactivate()
        {
            isActive = false;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sprite = isActive ? activeSprite : normalSprite;
        }
    }
}