using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;

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
        HandleMovement();
        HandleJump();
        HandleAttacks();
    }

    private void HandleMovement()
    {
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
    }

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

            // Aplique a gravidade
            velocity.y += gravity * Time.deltaTime;
        }

        // Aplicar movimento vertical
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAttacks()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (currentAttackTrigger == -1 && controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0) && !stateInfo.IsName("LightAttack") && !stateInfo.IsName("HeavyAttack") && !stateInfo.IsName("Capoeira"))
            {
                Debug.Log("Light Attack");
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

        if (stateInfo.normalizedTime >= 1.0f && currentAttackTrigger != -1)
        {
            currentAttackTrigger = -1;
            isAttacking = false;
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

    public void SubirNivel()
    {
        nivelAtual = Mathf.Clamp(nivelAtual + 1, 1, armasPorNivel.Length);
        AtivarArmaDoNivel();
    }
}
