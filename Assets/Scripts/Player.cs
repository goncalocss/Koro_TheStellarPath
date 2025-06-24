using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movimento e Física")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float rotationSpeed = 700f;

    private float attackEndTime = 0f;
    private float attackDurationBuffer = 0.1f;

    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private Vector3 horizontalMove;

    private bool isJumping = false;
    private bool doubleJumpUsed = false;
    public bool isAttacking = false;

    private int currentAttackTrigger = -1;

    public bool estaVivo = true;
    private bool jaMorreu = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        StartCoroutine(InputLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator InputLoop()
    {
        while (true)
        {
            if (estaVivo)
            {
                HandleMovement();
                HandleJump();
                HandleAttacks();
                ApplyMovement();
            }

            yield return null;
        }
    }

    private void HandleMovement()
    {
        if (isAttacking)
        {
            horizontalMove = Vector3.zero;
            animator.SetFloat("speed", 0f);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
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
            horizontalMove = moveDir * moveSpeed;

            float angle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            horizontalMove = Vector3.zero;
        }

        animator.SetFloat("speed", input.magnitude);
    }

    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            velocity.y = -0.5f;

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

                SoundManager.Instance.PlaySFX("jump");
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        Vector3 finalMove = horizontalMove;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleAttacks()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool inAttackAnim = stateInfo.IsName("LightAttack") || stateInfo.IsName("HeavyAttack") || stateInfo.IsName("Capoeira");

        if (currentAttackTrigger == -1 && controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0) && !inAttackAnim)
            {
                currentAttackTrigger = 0;
                animator.SetTrigger("LightAttack");
                isAttacking = true;
                attackEndTime = Time.time + stateInfo.length + attackDurationBuffer;
                SoundManager.Instance.PlaySFX("swing2");
            }
            else if (Input.GetMouseButtonDown(1) && !inAttackAnim)
            {
                currentAttackTrigger = 1;
                animator.SetTrigger("HeavyAttack");
                isAttacking = true;
                attackEndTime = Time.time + stateInfo.length + attackDurationBuffer;
                SoundManager.Instance.PlaySFX("swing3");
            }
        }

        if (currentAttackTrigger != -1 && Time.time >= attackEndTime)
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
        SoundManager.Instance.PlaySFX("player-death");

        Debug.Log("Player morreu!");
        GameManager.Instance.playerVivo = false;

        animator?.SetTrigger("Died");
        controller.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        PlayerMovement movimento = GetComponent<PlayerMovement>();
        if (movimento != null) movimento.enabled = false;

        StartCoroutine(EsperarEReiniciar());
    }

    private IEnumerator EsperarEReiniciar()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }

    public void ResetarEstado()
    {
        jaMorreu = false;
        estaVivo = true;
        velocity = Vector3.zero;

        if (animator != null)
        {
            if (!animator.enabled)
            {
                animator.enabled = true;
                Debug.Log("🎥 Animator reativado no respawn.");
            }

            animator.ResetTrigger("Died");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("LightAttack");
            animator.ResetTrigger("HeavyAttack");

            animator.Play("Idle", 0, 0f);
            Debug.Log("🎬 Triggers limpos e animação Idle forçada.");
        }

        GameManager.Instance.playerVivo = true;
        Debug.Log("✅ playerVivo = true no GameManager");
    }
}
