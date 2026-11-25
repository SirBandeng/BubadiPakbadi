using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using HorrorSchool.Interaction;
using HorrorSchool.Player;

namespace HorrorSchool.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Fade UI")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.35f;

        private bool isTransitioning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        private void OnEnable()
        {
            SameSceneDoor.OnSameSceneTeleport += HandleSameSceneTeleport;
            SceneTransitionDoor.OnCrossSceneTransition += HandleCrossSceneTransition;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SameSceneDoor.OnSameSceneTeleport -= HandleSameSceneTeleport;
            SceneTransitionDoor.OnCrossSceneTransition -= HandleCrossSceneTransition;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void HandleSameSceneTeleport(Transform playerTransform, Vector3 targetPosition, System.Action onMidFade)
        {
            if (isTransitioning) return;
            StartCoroutine(SameSceneRoutine(playerTransform, targetPosition, onMidFade));
        }

        private IEnumerator SameSceneRoutine(Transform playerTransform, Vector3 targetPosition, System.Action onMidFade)
        {
            isTransitioning = true;
            var player = playerTransform.GetComponent<PlayerController>();
            if (player != null) player.SetMovementEnabled(false);

            yield return StartCoroutine(Fade(1f));
            playerTransform.position = targetPosition;
            onMidFade?.Invoke();
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(Fade(0f));

            if (player != null) player.SetMovementEnabled(true);
            isTransitioning = false;
        }

        private void HandleCrossSceneTransition(string targetSceneName, string targetSpawnID)
        {
            if (isTransitioning) return;
            StartCoroutine(CrossSceneRoutine(targetSceneName, targetSpawnID));
        }

        private IEnumerator CrossSceneRoutine(string targetSceneName, string targetSpawnID)
        {
            isTransitioning = true;
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) player.SetMovementEnabled(false);

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.NextSpawnPointID = targetSpawnID;
            }

            yield return StartCoroutine(Fade(1f));
            SceneManager.LoadScene(targetSceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (GameStateManager.Instance != null && !string.IsNullOrEmpty(GameStateManager.Instance.NextSpawnPointID))
            {
                string targetID = GameStateManager.Instance.NextSpawnPointID;
                SpawnPoint[] points = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

                foreach (var point in points)
                {
                    if (point.SpawnID == targetID)
                    {
                        var player = FindFirstObjectByType<PlayerController>();
                        if (player != null)
                        {
                            player.transform.position = point.transform.position;
                        }
                        break;
                    }
                }

                GameStateManager.Instance.NextSpawnPointID = string.Empty;
            }

            if (isTransitioning)
            {
                StartCoroutine(FinishCrossSceneFade());
            }
        }

        private IEnumerator FinishCrossSceneFade()
        {
            yield return StartCoroutine(Fade(0f));
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) player.SetMovementEnabled(true);
            isTransitioning = false;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (fadeCanvasGroup == null) yield break;

            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
            float startAlpha = fadeCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = targetAlpha;
        }
    }
}