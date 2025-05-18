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

        target = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead)
        {
            SetTrigger("Defeated");
            return;
        }

        if (target == null || !GameManager.Instance.playerVivo)
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

            if (healthBar != null && healthBar.gameObject.activeSelf)
            {
                healthBar.Hide();
                Debug.Log("Barra de vida do boss desativada.");

                currentHealth = maxHealth;
                healthBar.SetMaxHealth(maxHealth);
                Debug.Log("Barra de vida do boss reiniciada.");
            }
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
            podeAtacar = true;
            Debug.Log("Área de ataque ativada.");
            Invoke(nameof(DesativarAreaDeAtaque), 0.5f);
        }
    }

    void DesativarAreaDeAtaque()
    {
        if (ataqueArea != null)
        {
            ataqueArea.SetActive(false);
            Debug.Log("Área de ataque desativada.");
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!podeAtacar || isDead) return;

        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Boss colidiu com a hitbox do jogador (pré-impacto).");

            // Guarda referência para depois aplicar dano
            StartCoroutine(AplicarDanoComDelay(2f));
            podeAtacar = false;
        }
    }

    private System.Collections.IEnumerator AplicarDanoComDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isDead) yield break;

        // Verifica se o jogador ainda está vivo e perto
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackDistance && GameManager.Instance.playerVivo)
        {
            Debug.Log("Boss causou dano ao jogador (no momento certo).");
            GameManager.Instance.ReceberDanoBoss();
        }

        // Reativa ataque depois do ciclo
        Invoke(nameof(ResetarAtaque), 0.1f);
    }

    public void ReeniciarDetatacaoBoos()
    {
        Transform novoAlvo = GameObject.FindWithTag("PlayerHitbox")?.transform;

        if (novoAlvo != null)
        {
            target = novoAlvo;
            Debug.Log("Novo alvo definido para o boss.");
        }
        else
        {
            Debug.LogWarning("Novo alvo não encontrado.");
        }
    }



}
