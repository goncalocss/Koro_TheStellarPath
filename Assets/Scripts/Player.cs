using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;

    public Transform cameraTransform;         // Referência à câmara
    public GameObject[] armasPorNivel;        // Armas já presentes no modelo do jogador
    public int nivelAtual = 1;                // Nível do jogador (1 = primeira arma)

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isJumping = false;
    private bool doubleJumpUsed = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        AtivarArmaDoNivel();
    }

    void Update()
    {
        // Movimento
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        if (input.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.z + camRight * input.x).normalized;

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            transform.forward = moveDir;
        }

        animator.SetFloat("speed", input.magnitude);

        // Saltos
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
            if (Input.GetButtonDown("Jump") && isJumping && !doubleJumpUsed)
            {
                velocity.y = jumpForce;
                doubleJumpUsed = true;

                animator.SetBool("isJumping", false);
                animator.SetBool("isDoubleJumping", true);
            }

            velocity.y += gravity * Time.deltaTime;
        }

        // Aplicar movimento vertical
        controller.Move(velocity * Time.deltaTime);

        // Ataques
        if (Input.GetMouseButtonDown(0) && !animator.GetBool("isAttacking") && controller.isGrounded)
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(ResetBool("isAttacking", 0.6f));
        }

        if (Input.GetMouseButtonDown(1) && !animator.GetBool("isHeavyAttacking") && controller.isGrounded)
        {
            animator.SetBool("isHeavyAttacking", true);
            StartCoroutine(ResetBool("isHeavyAttacking", 0.9f));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && controller.isGrounded)
        {
            animator.SetBool("isCapoeira", true);
            StartCoroutine(ResetBool("isCapoeira", 10.0f)); // ajusta o tempo à duração da animação
        }
    }

    private void AtivarArmaDoNivel()
    {
        for (int i = 0; i < armasPorNivel.Length; i++)
        {
            if (armasPorNivel[i] != null)
                armasPorNivel[i].SetActive(i == nivelAtual - 1);
        }
    }

    private IEnumerator ResetBool(string parameterName, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(parameterName, false);
    }

    // ⚡ Chama esta função se quiseres mudar de arma a meio do jogo
    public void SubirNivel()
    {
        nivelAtual = Mathf.Clamp(nivelAtual + 1, 1, armasPorNivel.Length);
        AtivarArmaDoNivel();
    }
}
