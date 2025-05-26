using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class BossAI : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 60f;
    public float attackDistance = 2f;
    public float moveSpeed = 3f;
    public int maxHealth = 300;
    private int currentHealth;

    public BossHealthBar healthBar;
    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;
    private string currentState = "";

    public GameObject ataqueArea;
    private bool podeAtacar = true;
    
    
    [Header("Cena pós-boss")]
    public string cenaCutscene;  // Define esta variável no Inspector


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

        StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        while (!isDead)
        {
            if (target == null || !GameManager.Instance.playerVivo)
            {
                ChangeState("Idle");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackDistance)
            {
                ChangeState("InAction");
            }
            else if (distance <= chaseDistance)
            {
                ChangeState("Running");
                ChaseTarget();

                if (healthBar != null && !healthBar.gameObject.activeSelf)
                {
                    healthBar.Show();
                    Debug.Log("Barra de vida do boss ativada.");

                    SoundManager.Instance.PlayMusic("boss-music1");
                }
            }
            else
            {
                ChangeState("Idle");

                if (healthBar != null && healthBar.gameObject.activeSelf)
                {
                    healthBar.Hide();
                    currentHealth = maxHealth;
                    healthBar.SetMaxHealth(maxHealth);
                    Debug.Log("Barra de vida do boss reiniciada.");

                    SoundManager.Instance.StopMusic();
                    SoundManager.Instance.PlayMusic("Wolrd1");
                }
            }

            yield return new WaitForSeconds(0.2f); // Mais leve que Update()
        }

        ChangeState("Defeated");
    }

    void ChaseTarget()
    {
        if (target == null) return;

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
            if (healthBar != null)
                healthBar.Hide();

            rb.isKinematic = true;
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            SoundManager.Instance.PlaySFX("boss-dying");
            SoundManager.Instance.StopMusic();

            Destroy(gameObject, 3f);
            Debug.Log("Boss derrotado!");


            //apos o boss desaparecer a cena muda 
            Invoke(nameof(MudarParaCenaCutscene), 3f); // Trocar cena após 3 segundos
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

    void ChangeState(string newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        SetTrigger(newState);
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
            StartCoroutine(AplicarDanoComDelay(2f));
            podeAtacar = false;
        }
    }

    private IEnumerator AplicarDanoComDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isDead || target == null) yield break;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackDistance && GameManager.Instance.playerVivo)
        {
            Debug.Log("Boss causou dano ao jogador (no momento certo).");
            GameManager.Instance.ReceberDanoBoss();
        }

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


    void MudarParaCenaCutscene()
    {
        if (!string.IsNullOrEmpty(cenaCutscene))
        {
            SceneManager.LoadScene(cenaCutscene);
        }
        else
        {
            Debug.LogWarning("Cena de cutscene não definida!");
        }
    }
}
