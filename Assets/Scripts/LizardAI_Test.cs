using UnityEngine;

public class LizardAI_Test : MonoBehaviour
{
    public Transform target; 
    public float chaseDistance = 10f;
    public float attackDistance = 0.8f;
    public int health = 100;
    public float moveSpeed = 2f; // Velocidade do inimigo

    private Animator animator;
    private bool isDead = false;
    private string currentState = "Idle"; // Estado atual do inimigo

    private float lastDistance = Mathf.Infinity; // Variável para armazenar a distância anterior

    void Start()
    {
        animator = GetComponent<Animator>();
        SetState("Idle");
    }

    void Update()
    {
        if (isDead || target == null) return;

        // Calculando a distância apenas uma vez
        float distance = Vector3.Distance(transform.position, target.position);
        Debug.Log("Distância para o jogador: " + distance); // Log para depuração

        // Evitar recalcular ou realizar transições desnecessárias
        if (distance != lastDistance)
        {
            lastDistance = distance;

            // Se o inimigo está dentro do alcance de ataque
            if (distance <= attackDistance)
            {
                SetState("InAction");
                moveSpeed = 0f; // Parar o inimigo enquanto ataca
            }
            else if (distance <= chaseDistance)
            {
                SetState("Running");
                moveSpeed = 2f; // Define a velocidade de corrida (ajuste conforme necessário)
                ChaseTarget();
            }
            else
            {
                SetState("Idle");
                moveSpeed = 0f; // O inimigo para de se mover quando está em idle
            }
        }

        // Atualiza o valor do parâmetro 'speed' no Animator
        animator.SetFloat("speed", moveSpeed);
    }

    void SetState(string newState)
    {
        if (newState == currentState) return;

        currentState = newState;

        // Limpeza das transições de animação
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Running");
        animator.ResetTrigger("InAction");
        animator.ResetTrigger("Defeated");

        // Define o novo estado de animação
        animator.SetTrigger(newState);
    }

    void ChaseTarget()
    {
        // Calcular direção e mover
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Impede movimentação vertical

        // Movimento suave com `MoveTowards`
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Atualiza a direção do inimigo para olhar para o jogador
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        // Log para depuração para ver se o inimigo está se movendo
        Debug.Log("Movendo-se para: " + target.position);
    }

    public void ReceberDano(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            animator.SetTrigger("Defeated");

            Collider col = GetComponent<Collider>();
            if (col) col.enabled = false;

            Destroy(gameObject, 2.5f);

            Debug.Log("FALECEU");
        }
    }
}
