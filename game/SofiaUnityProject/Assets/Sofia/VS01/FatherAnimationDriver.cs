using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;

namespace Sofia.VS01
{
    /// <summary>
    /// Bridges the physics motor to the visual Animator. It never writes to the
    /// Rigidbody2D or gameplay Transform.
    /// </summary>
    public sealed class FatherAnimationDriver : MonoBehaviour
    {
        public FatherMotor Motor;
        public Animator VisualAnimator;
        public Transform FacingRoot;
        public IKManager2D IkManager;
        public LimbSolver2D ReachSolver;
        public Transform ReachTarget;

        int previousJumpCount;
        bool previousGrounded;
        bool lookingAtHele;
        bool wasRunning;
        float facing = 1f;
        const float RunStartSpeed = 3.55f;
        const float RunStopSpeed = 2.55f;

        void Awake()
        {
            if (!Motor) Motor = GetComponent<FatherMotor>();
            if (!VisualAnimator) VisualAnimator = GetComponentInChildren<Animator>();
            if (!FacingRoot && VisualAnimator) FacingRoot = VisualAnimator.transform.Find("Facing");
            previousJumpCount = Motor ? Motor.JumpCount : 0;
            previousGrounded = Motor && Motor.Grounded;
            if (VisualAnimator) VisualAnimator.applyRootMotion = false;
            if (IkManager) IkManager.weight = 0f;
            if (ReachSolver) ReachSolver.weight = 1f;
        }

        void Update()
        {
            if (!Motor || !VisualAnimator) return;

            Vector2 velocity = Motor.Body ? Motor.Body.linearVelocity : Vector2.zero;
            float horizontalSpeed = Mathf.Abs(velocity.x);
            VisualAnimator.SetFloat("Speed", horizontalSpeed, .12f, Time.deltaTime);
            VisualAnimator.SetBool("IsGrounded", Motor.Grounded);
            VisualAnimator.SetBool("IsFalling", !Motor.Grounded && velocity.y < -.12f);
            VisualAnimator.SetBool("LookHele", lookingAtHele && Motor.Grounded);

            if (!Motor.Grounded)
            {
                wasRunning = false;
            }
            else if (!wasRunning && horizontalSpeed >= RunStartSpeed)
            {
                VisualAnimator.ResetTrigger("RunStop");
                VisualAnimator.SetTrigger("RunStart");
                wasRunning = true;
            }
            else if (wasRunning && horizontalSpeed <= RunStopSpeed)
            {
                VisualAnimator.ResetTrigger("RunStart");
                VisualAnimator.SetTrigger("RunStop");
                wasRunning = false;
            }

            if (Motor.JumpCount > previousJumpCount)
            {
                VisualAnimator.ResetTrigger("Land");
                VisualAnimator.SetTrigger("Jump");
                previousJumpCount = Motor.JumpCount;
            }

            if (!previousGrounded && Motor.Grounded)
            {
                VisualAnimator.ResetTrigger("Jump");
                VisualAnimator.SetTrigger("Land");
            }
            previousGrounded = Motor.Grounded;

            float direction = Mathf.Abs(Motor.Axis) > .08f
                ? Mathf.Sign(Motor.Axis)
                : Mathf.Abs(velocity.x) > .08f ? Mathf.Sign(velocity.x) : facing;
            if (Mathf.Abs(Motor.Axis) > .08f && direction != facing)
                VisualAnimator.SetTrigger("Turn");

            facing = direction;
            if (FacingRoot)
            {
                Vector3 scale = FacingRoot.localScale;
                scale.x = Mathf.Abs(scale.x) * facing;
                FacingRoot.localScale = scale;
            }
        }

        public void SetLookAtHele(bool value) => lookingAtHele = value;

        public void RequestReach(Transform target, float weight = 1f)
        {
            ReachTarget = target;
            if (ReachSolver)
            {
                ReachSolver.GetChain(0).target = target;
                ReachSolver.Initialize();
            }
            if (IkManager) IkManager.weight = target ? Mathf.Clamp01(weight) : 0f;
        }

        public void ReleaseReach()
        {
            if (IkManager) IkManager.weight = 0f;
        }

        public void SkipAwakening()
        {
            if (!VisualAnimator) return;
            VisualAnimator.Play("Locomotion", 0, 0f);
            VisualAnimator.Update(0f);
            wasRunning = false;
        }
    }
}
