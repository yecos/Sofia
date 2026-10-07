using UnityEngine;

namespace Sofia.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpVelocity = 10f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask = ~0;
        private Rigidbody2D body;
        private bool jumpQueued;

        private void Awake() => body = GetComponent<Rigidbody2D>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                jumpQueued = true;
        }

        private void FixedUpdate()
        {
            float axis = Input.GetAxisRaw("Horizontal");
            body.linearVelocity = new Vector2(axis * moveSpeed, body.linearVelocity.y);
            if (jumpQueued && IsGrounded())
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpVelocity);
            jumpQueued = false;
        }

        private bool IsGrounded()
        {
            Vector2 point = groundCheck ? groundCheck.position : (Vector2)transform.position + Vector2.down * 0.55f;
            return Physics2D.OverlapCircle(point, 0.12f, groundMask) != null;
        }
    }
}
