using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HorrorSchool.Core;
using HorrorSchool.Interaction;
using HorrorSchool.UI;

namespace HorrorSchool.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody2D rb;
        private InputAction moveAction;
        private InputAction interactAction;
        private Vector2 moveInput;
        private bool canMove = true;

        private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
        private IInteractable currentFocusedInteractable;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            var playerMap = inputActions.FindActionMap("Player", true);
            moveAction = playerMap.FindAction("Move", true);
            interactAction = playerMap.FindAction("Interact", true);
        }

        private void OnEnable()
        {
            moveAction.Enable();
            interactAction.Enable();
            interactAction.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            interactAction.performed -= OnInteractPerformed;
            moveAction.Disable();
            interactAction.Disable();
        }

        private void Update()
        {
            if (!canMove)
            {
                moveInput = Vector2.zero;
                return;
            }

            moveInput = moveAction.ReadValue<Vector2>();
            HandleFlip();
            UpdateClosestInteractable();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        private void HandleFlip()
        {
            if (spriteRenderer == null) return;

            if (moveInput.x > 0.05f)
                spriteRenderer.flipX = false;
            else if (moveInput.x < -0.05f)
                spriteRenderer.flipX = true;
        }

        private void UpdateClosestInteractable()
        {
            IInteractable closest = null;
            float minDistance = float.MaxValue;

            for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
            {
                var item = nearbyInteractables[i];
                if (item == null || !item.CanInteract) continue;

                MonoBehaviour mb = item as MonoBehaviour;
                if (mb == null) continue;

                float dist = Vector2.Distance(transform.position, mb.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = item;
                }
            }

            if (closest != currentFocusedInteractable)
            {
                currentFocusedInteractable?.ShowPrompt(false);
                currentFocusedInteractable = closest;
                currentFocusedInteractable?.ShowPrompt(true);
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (!canMove || currentFocusedInteractable == null) return;

            // Cegah interaksi ulang pada frame yang sama saat dialog baru ditutup
            if (DialogueManager.Instance != null &&
                (DialogueManager.Instance.IsDialogueActive || DialogueManager.Instance.WasClosedThisFrame))
            {
                return;
            }

            currentFocusedInteractable.Interact(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                if (!nearbyInteractables.Contains(interactable))
                {
                    nearbyInteractables.Add(interactable);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.ShowPrompt(false);
                nearbyInteractables.Remove(interactable);
                if (currentFocusedInteractable == interactable)
                {
                    currentFocusedInteractable = null;
                }
            }
        }

        public bool HasItem(string itemID)
        {
            return GameStateManager.Instance != null && GameStateManager.Instance.HasItem(itemID);
        }

        public void SetMovementEnabled(bool enabled)
        {
            canMove = enabled;
            if (!enabled && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}