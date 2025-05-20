using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    // Definir valores de movimento e física
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float rotationSpeed = 700f;  // Para melhorar a rotação

    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isJumping = false;
    private bool doubleJumpUsed = false;
    public bool isAttacking = false;

    private int currentAttackTrigger = -1;

    public bool estaVivo = true;
    private bool jaMorreu = false;

    // Inicializar componentes
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();



        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

    }

    // Atualizar estado a cada frame
    void Update()
    {
        if (!estaVivo) return; // ← Bloqueia qualquer execução pós-morte

        HandleMovement();
        HandleJump();
        HandleAttacks();
    }

    // Lidar com movimento e rotação
    private void HandleMovement()
    {
        if (isAttacking) return;  // Bloquear movimento enquanto atacando

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        // Movimento só é realizado se houver input
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

            // Rotacionar o personagem para o movimento de direção
            float angle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetFloat("speed", input.magnitude);
    }

    // Lidar com saltos e gravidade
    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            velocity.y = -2f;

            if (isJumping || doubleJumpUsed)
            {
                isJumping = false;
                doubleJumpUsed = false;
            }

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("Salto executado!");
                velocity.y = jumpForce;
                isJumping = true;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    // Lidar com ataques
    private void HandleAttacks()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Evitar que o jogador ataque enquanto está atacando ou em animações de ataque
        if (currentAttackTrigger == -1 && controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0) && !stateInfo.IsName("LightAttack") && !stateInfo.IsName("HeavyAttack") && !stateInfo.IsName("Capoeira"))
            {
                currentAttackTrigger = 0;
                animator.SetTrigger("LightAttack");
                isAttacking = true;
            }

            if (Input.GetMouseButtonDown(1) && !stateInfo.IsName("LightAttack") && !stateInfo.IsName("HeavyAttack") && !stateInfo.IsName("Capoeira"))
            {
                currentAttackTrigger = 1;
                animator.SetTrigger("HeavyAttack");
                isAttacking = true;
            }

        }

        // Reajustar após o ataque
        if (stateInfo.normalizedTime >= 1.0f && currentAttackTrigger != -1)
        {
            currentAttackTrigger = -1;
            isAttacking = false;
        }
    }

    public void Morrer()
    {
        if (jaMorreu) return;
        jaMorreu = true;
        estaVivo = false;

        Debug.Log("Player morreu!");

        GameManager.Instance.playerVivo = false;

        animator?.SetTrigger("Died");
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        var movimento = GetComponent<PlayerMovement>();
        if (movimento != null) movimento.enabled = false;

        StartCoroutine(EsperarEReiniciar());
    }

    private IEnumerator EsperarEReiniciar()
    {
        yield return new WaitForSeconds(3f); // Espera 2,5 segundos
                                             //mudar de cena
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);

    }

    public void ResetarEstado()
    {
        jaMorreu = false;
        estaVivo = true; // ← Fundamental para reativar Update()
        velocity = Vector3.zero; // ← Limpa impulso de queda

        if (animator != null)
        {
            if (!animator.enabled)
            {
                animator.enabled = true;
                Debug.Log("🎥 Animator reativado no respawn.");
            }

            // Limpa todos os triggers de animação
            animator.ResetTrigger("Died");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("DoubleJump");
            animator.ResetTrigger("LightAttack");
            animator.ResetTrigger("HeavyAttack");

            // Garante que o estado Idle entra imediatamente
            animator.Play("Idle", 0, 0f); // ← Nome do estado Idle no Animator
            Debug.Log("🎬 Triggers limpos e animação Idle forçada.");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerVivo = true;
            Debug.Log("✅ playerVivo = true no GameManager");
        }
    }






}
