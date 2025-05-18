using UnityEngine;

public class LizardAI_Test : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 10f;
    public float attackDistance = 0.8f;
    public float moveSpeed = 2f;
    public int health = 100;

    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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

        // ← Aqui usamos diretamente GameManager.Instance em vez da variável
        if (target == null || GameManager.Instance == null || !GameManager.Instance.playerVivo)
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
        }
        else
        {
            SetTrigger("Idle");
        }
    }

    void SetTrigger(string triggerName)
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Running");
        animator.ResetTrigger("InAction");
        animator.ResetTrigger("Defeated");
        animator.SetTrigger(triggerName);
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

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            SetTrigger("Defeated");

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, 2.5f);
            Debug.Log("O lagarto morreu!");
        }
    }

    public void ReiniciarDeteccao()
    {
        Transform novoAlvo = GameObject.FindWithTag("Player")?.transform;

        if (novoAlvo != null)
        {
            this.target = novoAlvo;
            Debug.Log("🎯 Alvo do inimigo atualizado após respawn.");
        }
    }
}
