using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorrorSchool.Interaction;
using HorrorSchool.UI;

namespace HorrorSchool.Puzzle
{
    public class LibraryBookcasePuzzle : PuzzleManagerBase
    {
        [Header("Rak Buku Target")]
        [SerializeField] private Transform slidingBookcase;
        [SerializeField] private Vector3 solvedPositionOffset = new Vector3(2.5f, 0f, 0f);
        [SerializeField] private float slideSpeed = 1.5f;

        [Header("Susunan Simbol di Scene")]
        [SerializeField] private List<InteractiveSymbolObject> symbolsInOrder; 
        // Wajib dimasukkan secara urut di Inspector: Index 0=JAM, 1=LILIN, 2=BULAN, 3=KUNCI

        private readonly List<string> inputSequence = new List<string>();
        private Vector3 originalPosition;
        private Vector3 targetPosition;
        private bool isMoving = false;

        private void OnEnable()
        {
            InteractiveSymbolObject.OnSymbolStateChanged += HandleSymbolPress;
        }

        private void OnDisable()
        {
            InteractiveSymbolObject.OnSymbolStateChanged -= HandleSymbolPress;
        }

        protected override void Start()
        {
            base.Start();
            if (slidingBookcase != null)
            {
                originalPosition = slidingBookcase.position;
                targetPosition = originalPosition + solvedPositionOffset;
            }
        }

        private void HandleSymbolPress(string symbolID, bool isActive)
        {
            if (isSolved || isMoving) return;

            if (isActive)
            {
                inputSequence.Add(symbolID);
                CheckSequence();
            }
            else
            {
                // Jika dinonaktifkan, hapus dari list
                inputSequence.Remove(symbolID);
            }
        }

        private void CheckSequence()
        {
            // 1. Cek apakah panjang input sudah menyamai jumlah simbol kunci (4)
            if (inputSequence.Count < symbolsInOrder.Count) return;

            bool isCorrect = true;
            for (int i = 0; i < symbolsInOrder.Count; i++)
            {
                if (inputSequence[i] != symbolsInOrder[i].SymbolID)
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                SolvePuzzle();
            }
            else
            {
                // Salah urutan: Reset semua tombol dan kosongkan input
                StartCoroutine(ResetPuzzleSequence());
            }
        }

        private IEnumerator ResetPuzzleSequence()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowSingleLine("Terdengar suara klik yang aneh... urutannya sepertinya salah.");
            }

            yield return new WaitForSeconds(1.5f);

            inputSequence.Clear();
            foreach (var symbol in symbolsInOrder)
            {
                symbol.ForceDeactivate();
            }
        }

        protected override void OnPuzzleSolved()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowSingleLine("Terdengar suara mekanisme kayu bergeser! Rak buku rahasia itu terbuka.");
            }
            StartCoroutine(SlideBookcaseRoutine());
        }

        protected override void OnAlreadySolved()
        {
            if (slidingBookcase != null)
            {
                slidingBookcase.position = targetPosition; // Langsung terbuka
            }
        }

        private IEnumerator SlideBookcaseRoutine()
        {
            isMoving = true;
            float elapsed = 0f;
            Vector3 startPos = slidingBookcase.position;

            while (elapsed < 2f)
            {
                elapsed += Time.deltaTime;
                slidingBookcase.position = Vector3.Lerp(startPos, targetPosition, (elapsed / 2f) * slideSpeed);
                yield return null;
            }

            slidingBookcase.position = targetPosition;
            isMoving = false;
        }
    }
}