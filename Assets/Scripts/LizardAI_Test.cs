using UnityEngine;

public class LizardAI_Test : MonoBehaviour
{
    public Transform target; // Referência ao macaco (player)
    public float chaseDistance = 10f;
    public float attackDistance = 0.8f;
    public int health = 100;
    public float moveSpeed = 2f;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetState("isIdle");
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            SetState("inAction");
        }
        else if (distance <= chaseDistance)
        {
            SetState("isRunning");
            ChaseTarget();
        }
        else
        {
            SetState("isIdle");
        }
    }

    void SetState(string state)
    {
        // Reset a todos os bools
        animator.SetBool("isIdle", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("inAction", false);
        animator.SetBool("isDefeated", false);

        // Ativar o estado atual
        animator.SetBool(state, true);
    }

    void ChaseTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // evitar movimentação vertical
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            animator.SetBool("isDefeated", true);

            Collider col = GetComponent<Collider>();
            if (col) col.enabled = false;

            Destroy(gameObject, 2.5f); // Tempo da animação de morte
        }
    }

    // Para testares no Editor (pressiona tecla para causar dano)
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 120, 30), "Dano ao lagarto"))
        {
            TakeDamage(50); // cada clique tira 50 de vida
        }
    }
}
