using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using HorrorSchool.Interaction;
using HorrorSchool.Player;

namespace HorrorSchool.UI
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;

        private string[] currentLines;
        private int currentIndex;
        private bool isDialogueActive;
        private int openedFrame = -1;
        private int lastClosedFrame = -1;

        private PlayerController cachedPlayer;

        public bool IsDialogueActive => isDialogueActive;
        public bool WasClosedThisFrame => Time.frameCount == lastClosedFrame;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        private void OnEnable()
        {
            InspectableObject.OnInspectRequested += StartDialogue;
            DoorBase.OnDoorFeedback += ShowSingleLine;
        }

        private void OnDisable()
        {
            InspectableObject.OnInspectRequested -= StartDialogue;
            DoorBase.OnDoorFeedback -= ShowSingleLine;
        }

        private void Update()
        {
            // Abaikan input pada frame yang sama saat dialog baru dibuka
            if (!isDialogueActive || Time.frameCount == openedFrame) return;

            if (Keyboard.current != null &&
                (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                AdvanceDialogue();
            }
        }

        public void ShowSingleLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            StartDialogue(new[] { line });
        }

        public void StartDialogue(string[] lines)
        {
            if (lines == null || lines.Length == 0 || isDialogueActive) return;

            currentLines = lines;
            currentIndex = 0;
            isDialogueActive = true;
            openedFrame = Time.frameCount;

            SetPlayerControl(false);

            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            if (dialogueText != null) dialogueText.text = currentLines[currentIndex];
        }

        private void AdvanceDialogue()
        {
            currentIndex++;
            if (currentIndex < currentLines.Length)
            {
                if (dialogueText != null) dialogueText.text = currentLines[currentIndex];
            }
            else
            {
                CloseDialogue();
            }
        }

        private void CloseDialogue()
        {
            isDialogueActive = false;
            lastClosedFrame = Time.frameCount;

            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            SetPlayerControl(true);
        }

        private void SetPlayerControl(bool enabled)
        {
            if (cachedPlayer == null)
            {
                cachedPlayer = FindFirstObjectByType<PlayerController>();
            }
            if (cachedPlayer != null)
            {
                cachedPlayer.SetMovementEnabled(enabled);
            }
        }
    }
}