using System;
using UnityEngine;
using HorrorSchool.Core;

namespace HorrorSchool.Interaction
{
    public class SameSceneDoor : DoorBase
    {
        [Header("Same-Scene Destination")]
        [SerializeField] private Transform destinationPoint;

        [Header("Camera Bounds Ruangan Tujuan")]
        [SerializeField] private bool updateCameraBounds = true;
        [SerializeField] private float newCameraMinX = 22f;
        [SerializeField] private float newCameraMaxX = 38f;

        public static event Action<Transform, Vector3, Action> OnSameSceneTeleport;

        protected override void UseDoor(Player.PlayerController player)
        {
            if (destinationPoint == null)
            {
                Debug.LogWarning($"DestinationPoint belum diisi pada {gameObject.name}");
                return;
            }

            Action onMidFade = () =>
            {
                if (updateCameraBounds && CameraFollow2D.Instance != null)
                {
                    CameraFollow2D.Instance.SetBounds(newCameraMinX, newCameraMaxX);
                }
            };

            if (OnSameSceneTeleport != null)
            {
                OnSameSceneTeleport.Invoke(player.transform, destinationPoint.position, onMidFade);
            }
            else
            {
                player.transform.position = destinationPoint.position;
                onMidFade.Invoke();
            }
        }
    }
}