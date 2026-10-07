using UnityEngine;

namespace Sofia.Hele
{
    public sealed class HeleFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(1.4f, 1.2f, 0f);
        [SerializeField] private float smoothTime = 0.25f;
        [SerializeField] private float hoverAmplitude = 0.12f;
        [SerializeField] private float hoverFrequency = 2.2f;
        private Vector3 velocity;
        private float time;

        public Transform Target { get => target; set => target = value; }

        private void LateUpdate()
        {
            if (!target) return;
            time += Time.deltaTime;
            Vector3 desired = target.position + offset + Vector3.up * (Mathf.Sin(time * hoverFrequency) * hoverAmplitude);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
