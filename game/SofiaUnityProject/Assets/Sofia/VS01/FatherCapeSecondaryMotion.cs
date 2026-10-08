using UnityEngine;

namespace Sofia.VS01
{
    /// <summary>
    /// Adds a damped visual follow-through to the skinned cape bones from motor
    /// velocity and acceleration. The physics body remains untouched.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public sealed class FatherCapeSecondaryMotion : MonoBehaviour
    {
        public Rigidbody2D Body;
        public Transform[] CapeBones;
        [Range(0f, 40f)] public float Spring = 17f;
        [Range(0f, 12f)] public float Damping = 5.5f;
        [Range(0f, 35f)] public float MaxAngle = 20f;

        Vector2 previousVelocity;
        float[] angles;
        float[] angularVelocities;

        public void Configure(Rigidbody2D body, Transform[] bones)
        {
            Body = body;
            CapeBones = bones;
            angles = new float[bones == null ? 0 : bones.Length];
            angularVelocities = new float[angles.Length];
            previousVelocity = body ? body.linearVelocity : Vector2.zero;
        }

        void LateUpdate()
        {
            if (!Body || CapeBones == null || CapeBones.Length == 0) return;
            float dt = Mathf.Clamp(Time.deltaTime, .0001f, .05f);
            Vector2 velocity = Body.linearVelocity;
            Vector2 acceleration = (velocity - previousVelocity) / dt;
            previousVelocity = velocity;

            for (int i = 0; i < CapeBones.Length; i++)
            {
                Transform bone = CapeBones[i];
                if (!bone) continue;
                float depth = (i + 1f) / CapeBones.Length;
                float target = Mathf.Clamp(
                    -velocity.x * 2.0f * depth - acceleration.x * .018f * depth,
                    -MaxAngle, MaxAngle);
                angularVelocities[i] += (target - angles[i]) * Spring * dt;
                angularVelocities[i] *= Mathf.Exp(-Damping * dt);
                angles[i] = Mathf.Clamp(angles[i] + angularVelocities[i] * dt, -MaxAngle, MaxAngle);

                // Animator writes the authored pose before LateUpdate, so this
                // composes a small inertial offset instead of replacing it.
                bone.localRotation *= Quaternion.Euler(0f, 0f, angles[i] * .08f);
            }
        }
    }
}
