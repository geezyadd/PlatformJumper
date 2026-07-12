using UnityEngine;
namespace platform_and_gems.CharacterMovement
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 5f; // Determines the horizontal speed of the character

        [Header("Jump Settings")]
        public float jumpForce = 10f; // Jump force
        public float CoyoteTime = 0.2f; // Duration of Coyote Time

        [Header("Ground Check")]
        public Transform GroundCheck; // Point to check if the character is on the ground
        public float GroundCheckRadius = 0.2f; // Size of the ground check area
        public LayerMask GroundLayer; // Layer that defines the ground

        private Rigidbody rb;
        private bool isGrounded;
        private float coyoteTimeCounter;
        private Animator animator;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            HandleMovement();
            HandleJump();
        }

        private void HandleMovement()
        {
            // Movement on the X axis
            float HorizontalmoveInput = Input.GetAxis("Horizontal");
            Vector3 moveVelocity = new Vector3(HorizontalmoveInput * MoveSpeed, rb.linearVelocity.y, 0f);
            rb.linearVelocity = moveVelocity;

            // Rotation control based on movement direction
            if (Mathf.Abs(HorizontalmoveInput) > 0.01f) // If there is movement
            {
                float targetYRotation = HorizontalmoveInput > 0 ? 180f : 0f; // 180 degrees for right, 0 degrees for left
                Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        private void HandleJump()
        {
            // Check if the character is on the ground
            isGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundLayer);

            // Counter for Coyote Time
            if (isGrounded)
            {
                coyoteTimeCounter = CoyoteTime;
                animator.SetBool("jump", false);
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // Check for jump input and coyote time
            if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f); // Reset current vertical velocity
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetBool("jump", true);
            }
        }
    }
}