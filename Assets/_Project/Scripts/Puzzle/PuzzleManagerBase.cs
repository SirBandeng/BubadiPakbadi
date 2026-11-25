using UnityEngine;

namespace HorrorSchool.Puzzle
{
    public abstract class PuzzleManagerBase : MonoBehaviour
    {
        [Header("Base Puzzle Settings")]
        [SerializeField] protected string puzzleID; // ID unik untuk GameStateManager
        [SerializeField] protected bool isSolved = false;

        public bool IsSolved => isSolved;

        protected virtual void Start()
        {
            // Jika game mendeteksi puzzle ini sudah pernah dipecahkan sebelumnya
            if (Core.GameStateManager.Instance != null && Core.GameStateManager.Instance.HasFlag(puzzleID))
            {
                isSolved = true;
                OnAlreadySolved();
            }
        }

        // Dipanggil saat puzzle pertama kali berhasil dipecahkan
        public virtual void SolvePuzzle()
        {
            if (isSolved) return;
            isSolved = true;

            if (Core.GameStateManager.Instance != null && !string.IsNullOrEmpty(puzzleID))
            {
                Core.GameStateManager.Instance.SetFlag(puzzleID);
            }

            OnPuzzleSolved();
        }

        protected abstract void OnPuzzleSolved();
        protected abstract void OnAlreadySolved();
    }
}