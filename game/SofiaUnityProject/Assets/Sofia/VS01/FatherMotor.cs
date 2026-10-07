using UnityEngine;
using UnityEngine.InputSystem;

namespace Sofia.VS01
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class FatherMotor : MonoBehaviour
    {
        public float walkSpeed = 2.3f, runSpeed = 5f, acceleration = 22f, deceleration = 28f;
        public float jumpSpeed = 9.4f, coyoteTime = .13f, inputBuffer = .13f;
        public bool ControlEnabled = true;
        public bool ExternalInput;
        public float Axis;
        public bool Running;
        public bool Grounded { get; private set; }
        public int JumpCount { get; private set; }
        public Rigidbody2D Body { get; private set; }
        float grace, buffered;
        BoxCollider2D shape;
        readonly RaycastHit2D[] hits = new RaycastHit2D[8];
        ContactFilter2D groundFilter;
        void Awake()
        {
            Body = GetComponent<Rigidbody2D>(); shape = GetComponent<BoxCollider2D>();
            groundFilter = new ContactFilter2D { useLayerMask = true, layerMask = 1 << 0, useTriggers = false };
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            Body.freezeRotation = true; Body.gravityScale = 1.8f;
        }
        public void QueueJump() => buffered = inputBuffer;
        void Update()
        {
            if (ExternalInput) return;
            var k = Keyboard.current;
            Axis = k == null ? 0 : ((k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0));
            Running = k != null && (k.leftShiftKey.isPressed || k.rightShiftKey.isPressed);
            if (k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame)) QueueJump();
        }
        void FixedUpdate()
        {
            Grounded = shape.Cast(Vector2.down, groundFilter, hits, .07f) > 0 && Body.linearVelocity.y <= .1f;
            grace = Grounded ? coyoteTime : grace - Time.fixedDeltaTime;
            float desired = ControlEnabled ? Axis * (Running ? runSpeed : walkSpeed) : 0;
            float vx = Mathf.MoveTowards(Body.linearVelocity.x, desired, (Mathf.Abs(desired) > .01f ? acceleration : deceleration) * Time.fixedDeltaTime);
            float vy = Body.linearVelocity.y;
            if (ControlEnabled && buffered > 0 && grace > 0)
            { vy = jumpSpeed; buffered = 0; grace = 0; JumpCount++; Grounded = false; }
            buffered -= Time.fixedDeltaTime;
            Body.linearVelocity = new Vector2(vx, Mathf.Max(vy, -14f));
        }
        public void Warp(Vector3 position)
        { Body.position = position; transform.position = position; Body.linearVelocity = Vector2.zero; buffered = grace = 0; }
    }
}
