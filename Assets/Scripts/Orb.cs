using UnityEngine;

public class Orb : MonoBehaviour
{
    private CollectiblesManager collectiblesManager; // Referência ao CollectiblesManager

    void Start()
    {
        // Encontre o CollectiblesManager na cena (usando FindObjectOfType)
        if (collectiblesManager == null)
        {
            collectiblesManager = FindFirstObjectByType<CollectiblesManager>(); // Usar FindFirstObjectByType para pegar a instância na cena
        }
    }

    // Quando a orb entra em contato com o jogador
    void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador colidiu com a orb
        if (other.CompareTag("Player"))
        {
            // Incrementa a contagem de orbs no CollectiblesManager
            collectiblesManager.IncrementOrbCount();

            // Log para depuração
            Debug.Log("Orb coletada. Total de orbs: " + collectiblesManager.GetOrbCount());

            // Destruir a orb depois que for coletada
            Destroy(gameObject);
        }
    }
}
