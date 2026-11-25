using UnityEngine;

namespace HorrorSchool.Core
{
    public class CameraFollow2D : MonoBehaviour
    {
        public static CameraFollow2D Instance { get; private set; }

        [Header("Target & Smoothing")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

        [Header("Batas Ruangan (X Axis)")]
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
            SnapToTarget();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            float desiredX = Mathf.Clamp(target.position.x + offset.x, minX, maxX);
            Vector3 desiredPos = new Vector3(desiredX, offset.y, offset.z);
            transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        }

        public void SetBounds(float newMinX, float newMaxX)
        {
            minX = newMinX;
            maxX = newMaxX;
            SnapToTarget();
        }

        public void SnapToTarget()
        {
            if (target == null) return;
            float clampedX = Mathf.Clamp(target.position.x + offset.x, minX, maxX);
            transform.position = new Vector3(clampedX, offset.y, offset.z);
        }
    }
}