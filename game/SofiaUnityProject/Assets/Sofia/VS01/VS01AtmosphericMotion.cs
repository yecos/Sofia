using UnityEngine;

namespace Sofia.VS01
{
    // Moves decorative depth layers without affecting collision or player physics.
    public sealed class VS01AtmosphericMotion : MonoBehaviour
    {
        public Transform CameraTarget;
        public Vector3 Origin;
        [Range(0, 1)] public float Parallax = .12f;
        public float DriftAmplitude = .35f;
        public float DriftRate = .24f;
        public float VerticalAmplitude = .08f;
        public float Phase;

        void Awake() { if (!CameraTarget && Camera.main) CameraTarget = Camera.main.transform; }

        void LateUpdate()
        {
            float t = Time.time * DriftRate + Phase;
            float cameraOffset = CameraTarget ? (CameraTarget.position.x - Origin.x) * Parallax : 0f;
            transform.position = Origin + new Vector3(cameraOffset + Mathf.Sin(t) * DriftAmplitude,
                Mathf.Cos(t * .73f) * VerticalAmplitude, 0);
        }
    }
}
