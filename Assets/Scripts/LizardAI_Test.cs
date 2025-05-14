using UnityEngine;

public class LizardAI_Test : MonoBehaviour
{
    public Transform target; // O alvo do inimigo (geralmente o jogador)
    public float chaseDistance = 10f; // Distância para perseguir o jogador
    public float attackDistance = 0.8f; // Distância para atacar o jogador
    public float moveSpeed = 2f; // Velocidade de movimento do inimigo
    public int health = 100; // Saúde do inimigo
    private Animator animator; // Para controlar as animações
    private Rigidbody rb; // Para controlar a física
    private bool isDead = false; // Se o inimigo está morto

    void Start()
    {
        animator = GetComponent<Animator>(); // Obtém o componente Animator
        rb = GetComponent<Rigidbody>(); // Obtém o componente Rigidbody
        SetTrigger("Idle"); // Define o estado inicial como Idle
    }

    void Update()
    {
        if (isDead || target == null) return; // Se o inimigo estiver morto ou não tiver alvo, não faz nada

        float distance = Vector3.Distance(transform.position, target.position); // Calcula a distância até o alvo

        // Comportamento baseado na distância
        if (distance <= attackDistance)
        {
            SetTrigger("InAction"); // Ativa a animação de ataque se o inimigo estiver perto o suficiente
        }
        else if (distance <= chaseDistance)
        {
            SetTrigger("Running"); // Ativa a animação de corrida se o inimigo estiver perto o suficiente para perseguir
            ChaseTarget(); // Chama o método para perseguir o jogador
        }
        else
        {
            SetTrigger("Idle"); // Caso contrário, mantém o inimigo em idle
        }


    }

    // Método para mudar de animação usando Triggers
    void SetTrigger(string triggerName)
    {
        // Resetando todos os triggers antes de ativar o novo
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Running");
        animator.ResetTrigger("InAction");
        animator.ResetTrigger("Defeated");

        // Ativa o trigger correspondente ao nome passado
        animator.SetTrigger(triggerName);

    }

    // Método para fazer o inimigo perseguir o jogador
    void ChaseTarget()
    {
        // Calcula a direção do movimento em direção ao jogador
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Impede o inimigo de subir ou descer (mantém a altura constante)

        // Move o inimigo em direção ao jogador com base na velocidade
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Atualiza a direção que o inimigo está olhando (rotação)
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    // Método para o inimigo receber dano (quando a saúde chegar a 0, ele morre)
    public void ReceberDano(int damage)
    {
        if (isDead) return; // Se já estiver morto, não faz nada

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            SetTrigger("Defeated"); // Ativa a animação de morte

            // Desabilitar o Rigidbody para parar os efeitos da física
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Desabilita a física (parando movimento)
            }

            // Desabilitar o Collider para que o inimigo não interaja com outros objetos
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false; // Desabilita a colisão
            }

            Destroy(gameObject, 2.5f); // Destroi o inimigo após 2,5 segundos

            //Metodo de release orbs



            Debug.Log("O lagarto morreu!");
        }
    }

}