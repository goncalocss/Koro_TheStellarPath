using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 15f;
    public float attackDistance = 2f;
    public float moveSpeed = 3f;
    public int maxHealth = 300;
    private int currentHealth;
    public BossHealthBar healthBar;
    public GameManager gameManager;
    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;

    public GameObject ataqueArea;
    private bool podeAtacar = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        SetTrigger("Idle");
    }

    void Update()
    {
        if (isDead || target == null || !gameManager.playerVivo)
        {
            SetTrigger("Idle");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            SetTrigger("InAction");
        }
        else if (distance <= chaseDistance)
        {
            SetTrigger("Running");
            ChaseTarget();

            if (healthBar != null && !healthBar.gameObject.activeSelf)
            {
                healthBar.Show();
                Debug.Log("Barra de vida do boss ativada.");
            }
        }
        else
        {
            SetTrigger("Idle");
        }
    }

    void ChaseTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    public void ReceberDano(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Boss recebeu dano: " + damage);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            SetTrigger("Defeated");

            if (healthBar != null)
                healthBar.Hide();

            rb.isKinematic = true;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Destroy(gameObject, 3f);
            Debug.Log("Boss derrotado!");
        }
    }

    public void AtivarAreaDeAtaque()
    {
        if (ataqueArea != null)
        {
            ataqueArea.SetActive(true);
            Debug.Log("Área de ataque ativada.");
            podeAtacar = false;
            Invoke(nameof(DesativarAreaDeAtaque), 0.5f);
        }
    }

    void DesativarAreaDeAtaque()
    {
        if (ataqueArea != null)
        {
            ataqueArea.SetActive(false);
            Debug.Log("Área de ataque desativada.");
            Invoke(nameof(ResetarAtaque), 1f);
        }
    }

    void ResetarAtaque()
    {
        podeAtacar = true;
        Debug.Log("Boss pode atacar novamente.");
    }

    void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Running");
        animator.ResetTrigger("InAction");
        animator.ResetTrigger("Defeated");

        animator.SetTrigger(triggerName);
        Debug.Log("Animação: " + triggerName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead || !podeAtacar) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Boss colidiu com o jogador.");

            if (gameManager != null)
            {
                Debug.Log("Boss causou dano ao jogador.");
                gameManager.ReceberDano();
            }

            AtivarAreaDeAtaque();
        }
    }
}
