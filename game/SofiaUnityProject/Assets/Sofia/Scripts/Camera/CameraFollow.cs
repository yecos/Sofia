using UnityEngine;

namespace Sofia.CameraSystem
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.18f;
        private Vector3 velocity;

        public Transform Target { get => target; set => target = value; }

        private void LateUpdate()
        {
            if (!target) return;
            Vector3 desired = new(target.position.x, target.position.y + 1.2f, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
