using UnityEngine;
using System.Collections;

public class LizardAI_Test : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 10f;
    public float attackDistance = 0.8f;
    public float moveSpeed = 2f;
    public int health = 100;

    private Animator animator;
    private Rigidbody rb;
    private Collider col;
    private bool isDead = false;
    private string currentState = "";

    private Coroutine chaseRoutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        target = GameObject.FindWithTag("Player")?.transform;

        ChangeState("Idle");
        StartCoroutine(StateControlLoop());
    }

    private IEnumerator StateControlLoop()
    {
        while (!isDead)
        {
            if (GameManager.Instance == null || !GameManager.Instance.playerVivo || target == null)
            {
                ChangeState("Idle");
                StopChase();
            }
            else
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackDistance)
                {
                    ChangeState("InAction");
                    StopChase();
                }
                else if (distance <= chaseDistance)
                {
                    ChangeState("Running");
                    StartChase();
                }
                else
                {
                    ChangeState("Idle");
                    StopChase();
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        ChangeState("Defeated");
    }

    private void StartChase()
    {
        if (chaseRoutine == null)
            chaseRoutine = StartCoroutine(ChaseLoop());
    }

    private void StopChase()
    {
        if (chaseRoutine != null)
        {
            StopCoroutine(chaseRoutine);
            chaseRoutine = null;
        }
    }

    private IEnumerator ChaseLoop()
    {
        while (!isDead && target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized;

            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            transform.LookAt(targetPosition);

            yield return null; // move a cada frame
        }
    }

    private void ChangeState(string newState)
    {
        if (newState == currentState) return;

        animator.ResetTrigger(currentState);
        currentState = newState;
        animator.SetTrigger(currentState);
    }

    public void ReceberDano(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            StopChase();
            ChangeState("Defeated");

            if (rb != null) rb.isKinematic = true;
            if (col != null) col.enabled = false;

            Destroy(gameObject, 2.5f);
#if UNITY_EDITOR
            Debug.Log("🦎 O lagarto morreu!");
#endif
        }
    }

    public void ReiniciarDeteccao()
    {
        Transform novoAlvo = GameObject.FindWithTag("Player")?.transform;

        if (novoAlvo != null)
        {
            target = novoAlvo;
#if UNITY_EDITOR
            Debug.Log("🎯 Alvo do inimigo atualizado após respawn.");
#endif
        }
    }
}
