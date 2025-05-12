using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inimigo"))
        {
            // Correto:
            LizardAI_Test inimigo = other.GetComponent<LizardAI_Test>();

            if (inimigo != null)
            {
                inimigo.ReceberDano(damage);
            }
        }
    }
}
