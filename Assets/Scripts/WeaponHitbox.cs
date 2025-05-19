using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 1;
    private Player player;
    private CollectiblesManager collectiblesManager;

    // Tags válidas para objetos interativos (sem dano)
    private readonly string[] objetosInterativos = { "Box", "Bau" };

    void Start()
    {
        player = FindObjectOfType<Player>();
        collectiblesManager = FindObjectOfType<CollectiblesManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player == null || !player.isAttacking) return;

        // 🎯 Lógica para inimigos normais
        if (other.CompareTag("Inimigo"))
        {
            if (other.TryGetComponent(out LizardAI_Test inimigo))
            {
                inimigo.ReceberDano(damage);
            }

            TentarAcionarColetavel(other);
            return;
        }

        // 🧠 Lógica para bosses
        if (other.CompareTag("Boss"))
        {
            if (other.TryGetComponent(out BossAI boss))
            {
                boss.ReceberDano(damage);
            }

            TentarAcionarColetavel(other);
            return;
        }

        // 📦 Outros objetos interativos (caixas, baús, etc.)
        if (System.Array.Exists(objetosInterativos, tag => other.CompareTag(tag)))
        {
            TentarAcionarColetavel(other);
        }
    }

    private void TentarAcionarColetavel(Collider other)
    {
        if (collectiblesManager != null)
        {
            collectiblesManager.HitBox(other);
        }
    }
}
