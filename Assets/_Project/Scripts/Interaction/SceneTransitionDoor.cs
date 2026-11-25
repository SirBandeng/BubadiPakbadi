using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorSchool.Interaction
{
    public class SceneTransitionDoor : DoorBase
    {
        [Header("Cross-Scene Destination")]
        [SerializeField] private string targetSceneName;
        [SerializeField] private string targetSpawnPointID; // ID posisi muncul Atop di scene tujuan

        public static event Action<string, string> OnCrossSceneTransition;

        protected override void UseDoor(Player.PlayerController player)
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning($"TargetSceneName kosong pada {gameObject.name}");
                return;
            }

            if (OnCrossSceneTransition != null)
            {
                OnCrossSceneTransition.Invoke(targetSceneName, targetSpawnPointID);
            }
            else
            {
                // Fallback pindah scene langsung untuk testing awal
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}