using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossMundo2AI : MonoBehaviour
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
    public string cenaCutscene;

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

            if (distance > chaseDistance)
            {
                ChangeState("Idle");

                if (healthBar != null && healthBar.gameObject.activeSelf)
                {
                    healthBar.Hide();
                    currentHealth = maxHealth;
                    healthBar.SetMaxHealth(maxHealth);
                    Debug.Log("Barra de vida do boss reiniciada.");

                    SoundManager.Instance.StopMusic();
                    string cenaAtual = SceneManager.GetActiveScene().name;
                    if (cenaAtual == "Verdalya")
                        SoundManager.Instance.PlayMusic("World1");
                    else if (cenaAtual == "Drexan")
                        SoundManager.Instance.PlayMusic("World2");
                    else if (cenaAtual == "Nebelya")
                        SoundManager.Instance.PlayMusic("World3");
                    else
                        Debug.LogWarning("Música não definida para a cena atual: " + cenaAtual);
                }
            }
            else if (distance > attackDistance)
            {
                ChangeState("Running");
                ChaseTarget();

                if (healthBar != null && !healthBar.gameObject.activeSelf)
                {
                    healthBar.Show();
                    Debug.Log("Barra de vida do boss ativada.");

                    string cenaAtual = SceneManager.GetActiveScene().name;
                    if (cenaAtual == "Verdalya")
                        SoundManager.Instance.PlayMusic("boss-music1");
                    else if (cenaAtual == "Drexan")
                        SoundManager.Instance.PlayMusic("boss-music2");
                    else if (cenaAtual == "Nebelya")
                        SoundManager.Instance.PlayMusic("boss-music3");
                    else
                        Debug.LogWarning("Música não definida para a cena atual: " + cenaAtual);
                }
            }
            else // <= attackDistance
            {
                ChangeState("InAction"); // ativa animação de ataque
            }

            yield return new WaitForSeconds(0.2f);
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
            healthBar.SetHealth(currentHealth, maxHealth);

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

            Invoke(nameof(MudarParaCenaCutscene), 3f);
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
        }
    }

    public void DesativarAreaDeAtaque() // chamado pela animação
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
        if (isDead || !podeAtacar) return;

        if (other.gameObject.CompareTag("PlayerHitbox") && ataqueArea != null && ataqueArea.activeSelf)
        {
            Debug.Log(">> ENTER: Boss colidiu com a hitbox do jogador.");
            StartCoroutine(ImpactoJogador());
            podeAtacar = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDead || !podeAtacar) return;

        if (other.gameObject.CompareTag("PlayerHitbox") && ataqueArea != null && ataqueArea.activeSelf)
        {
            Debug.Log(">> STAY: Boss continua colidindo com a hitbox do jogador.");
            StartCoroutine(ImpactoJogador());
            podeAtacar = false;
        }
    }

    private IEnumerator ImpactoJogador()
    {
        yield return null;

        if (isDead || target == null) yield break;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackDistance && GameManager.Instance.playerVivo)
        {
            Debug.Log("Boss causou dano ao jogador.");
            GameManager.Instance.ReceberDanoBoss();
        }

        Invoke(nameof(ResetarAtaque), 0.5f);
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
