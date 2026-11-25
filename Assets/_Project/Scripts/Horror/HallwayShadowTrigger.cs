using System.Collections;
using UnityEngine;
using HorrorSchool.Player;
using HorrorSchool.UI;

namespace HorrorSchool.Horror
{
    public class HallwayShadowTrigger : StoryTriggerBase
    {
        [Header("Shadow Visual Settings")]
        [SerializeField] private SpriteRenderer shadowRenderer;
        [SerializeField] private Vector3 moveOffset = new Vector3(2f, 0f, 0f);
        [SerializeField] private float vanishDuration = 1.4f;

        [Header("Monolog Atop Setelah Melihat Bayangan")]
        [TextArea(2, 4)]
        [SerializeField] private string[] reactionLines = new[]
        {
            "Tunggu... barusan ada orang berdiri di ujung lorong!",
            "Bara?! Apa itu kamu?!",
            "Dia berjalan ke arah perpustakaan..."
        };

        private void OnEnable()
        {
            if (shadowRenderer != null)
            {
                shadowRenderer.gameObject.SetActive(false);
            }
        }

        protected override void OnTriggered(PlayerController player)
        {
            StartCoroutine(ShadowSequenceRoutine(player));
        }

        private IEnumerator ShadowSequenceRoutine(PlayerController player)
        {
            player.SetMovementEnabled(false);

            if (shadowRenderer != null)
            {
                shadowRenderer.gameObject.SetActive(true);
                Color startColor = shadowRenderer.color;
                startColor.a = 0.85f;
                shadowRenderer.color = startColor;

                Vector3 startPos = shadowRenderer.transform.position;
                Vector3 endPos = startPos + moveOffset;

                float elapsed = 0f;
                while (elapsed < vanishDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / vanishDuration;

                    shadowRenderer.transform.position = Vector3.Lerp(startPos, endPos, t);
                    Color c = startColor;
                    c.a = Mathf.Lerp(0.85f, 0f, t);
                    shadowRenderer.color = c;

                    yield return null;
                }

                shadowRenderer.gameObject.SetActive(false);
            }

            player.SetMovementEnabled(true);

            if (DialogueManager.Instance != null && reactionLines.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(reactionLines);
            }
        }
    }
}