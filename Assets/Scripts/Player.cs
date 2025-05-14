using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    // Definir valores de movimento e física
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float rotationSpeed = 700f;  // Para melhorar a rotação

    public Transform cameraTransform;
    public GameObject[] armasPorNivel;
    public int nivelAtual = 1;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isJumping = false;
    private bool doubleJumpUsed = false;
    public bool isAttacking = false;

    private int currentAttackTrigger = -1;

    // Inicializar componentes
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        AtivarArmaDoNivel();
    }

    // Atualizar estado a cada frame
    void Update()
    {
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
                velocity.y = jumpForce;
                isJumping = true;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && isJumping && !doubleJumpUsed)
            {
                velocity.y = jumpForce;
                doubleJumpUsed = true;
                animator.SetTrigger("DoubleJump");
            }

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

            if (Input.GetKeyDown(KeyCode.LeftShift) && !stateInfo.IsName("LightAttack") && !stateInfo.IsName("HeavyAttack") && !stateInfo.IsName("Capoeira"))
            {
                animator.SetTrigger("Capoeira");
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

    // Ativar a arma do nível atual
    private void AtivarArmaDoNivel()
    {
        for (int i = 0; i < armasPorNivel.Length; i++)
        {
            if (armasPorNivel[i] != null)
                armasPorNivel[i].SetActive(i == nivelAtual - 1);
        }
    }

    // Subir de nível e ativar nova arma
    public void SubirNivel()
    {
        nivelAtual = Mathf.Clamp(nivelAtual + 1, 1, armasPorNivel.Length);
        AtivarArmaDoNivel();
    }
}
