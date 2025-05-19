using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        Player player = FindObjectOfType<Player>();

        if (player != null && player.isAttacking)
        {
            if (other.CompareTag("Inimigo"))
            {
                LizardAI_Test inimigo = other.GetComponent<LizardAI_Test>();

                if (inimigo != null)
                {
                    inimigo.ReceberDano(damage);
                }

                CollectiblesManager collectiblesManager = FindObjectOfType<CollectiblesManager>();
                if (collectiblesManager != null)
                {
                    collectiblesManager.HitBox(other);
                }
            }

            if (other.CompareTag("Boss"))
            {
                BossAI boss = other.GetComponent<BossAI>();

                if (boss != null)
                {
                    boss.ReceberDano(damage);
                }

                CollectiblesManager collectiblesManager = FindObjectOfType<CollectiblesManager>();
                if (collectiblesManager != null)
                {
                    collectiblesManager.HitBox(other);
                }
            }
        }

        if (other.CompareTag("Box"))
        {
            CollectiblesManager collectiblesManager = FindObjectOfType<CollectiblesManager>();
            if (collectiblesManager != null)
            {
                collectiblesManager.HitBox(other);
            }
        }

        if (other.CompareTag("Bau"))
        {
            CollectiblesManager collectiblesManager = FindObjectOfType<CollectiblesManager>();
            if (collectiblesManager != null)
            {
                collectiblesManager.HitBox(other);
            }
        }


    }
} 