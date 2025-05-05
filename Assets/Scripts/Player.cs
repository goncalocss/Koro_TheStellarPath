using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public Transform cameraTransform; // <-- Referência à câmara

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isJumping = false;
    private bool doubleJumpUsed = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Se não for atribuída manualmente
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        if (input.magnitude >= 0.1f)
        {
            // Movimento relativo à direção da câmara
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.z + camRight * input.x).normalized;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Rotaciona o player para a direção do movimento
            transform.forward = moveDir;
        }

        animator.SetFloat("speed", input.magnitude);

        // Jump
        if (controller.isGrounded)
        {
            velocity.y = -2f;
            if (isJumping || doubleJumpUsed)
            {
                isJumping = false;
                doubleJumpUsed = false;
                animator.SetBool("isJumping", false);
                animator.SetBool("isDoubleJumping", false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                isJumping = true;
                animator.SetBool("isJumping", true);
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && !doubleJumpUsed)
            {
                velocity.y = jumpForce;
                doubleJumpUsed = true;
                animator.SetBool("isDoubleJumping", true);
            }

            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
