using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    // Dano que a arma causa
    public int damage = 1;

    // Quando a arma entra em colisão com algum objeto
    private void OnTriggerEnter(Collider other)
    {
        // Log para ver se a colisão com a caixa é detectada
        Debug.Log("Colisão com: " + other.gameObject.name);

        if (other.CompareTag("Inimigo"))
        {
            LizardAI_Test inimigo = other.GetComponent<LizardAI_Test>();

            if (inimigo != null)
            {
                inimigo.ReceberDano(damage);
            }
        }

        if (other.CompareTag("Box"))
        {
            // Se a colisão for com a caixa, chama o método de destruição
            Debug.Log("Colisão com a caixa detectada!");
            CollectiblesManager collectiblesManager = FindObjectOfType<CollectiblesManager>();
            if (collectiblesManager != null)
            {
                collectiblesManager.HitBox(other);
            }
        }
    }
}
