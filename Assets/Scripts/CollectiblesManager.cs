using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    public GameObject orbPrefab; // Prefab da orb
    public Transform player; // Referência ao player

    // Distância de interação para o ataque
    public float interactionDistance = 3f;
    private int orbCount = 0;

    private void Update()
    {
        // Chama a função para puxar as orbs se houver um impacto
        PullOrbs();
    }

    // Função chamada para destruir a caixa e soltar orbs
    public void HitBox(Collider other)
    {
        Debug.Log("HitBox chamado para: " + other.gameObject.name);

        if (other.CompareTag("Box"))
        {
            // Destrói a caixa
            Destroy(other.gameObject);

            // Libera orbs (geração aleatória de orbs ao redor)
            ReleaseOrbs(other.transform.position);
        }
        else if (other.CompareTag("Inimigo"))
        {
            Debug.Log("Inimigo atingido e orbs dropadas: " + other.gameObject.name);

            // Chama ReleaseOrbs após 2,5 segundos
            Invoke("DropOrbs", 1.5f);
        }
    }


    // Função para soltar orbs
    private void ReleaseOrbs(Vector3 position)
    {
        int numOrbs = Random.Range(3, 6);
        for (int i = 0; i < numOrbs; i++)
        {
            Vector3 spawnPosition = position + new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f));
            GameObject orb = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);

            orb.tag = "Orb";

            orb.AddComponent<OrbMovement>();
        }
    }

    private void PullOrbs()
    {
        // Encontra todas as orbs no jogo
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        foreach (var orb in orbs)
        {
            // Puxa as orbs em direção ao player
            if (!orb.GetComponent<OrbMovement>().isCollected)  // Se a orb não foi coletada ainda
            {
                Vector3 direction = (player.position - orb.transform.position).normalized;
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, player.position, 0.1f); // Ajuste de velocidade
            }
        }
    }

    // Método para incrementar o número de orbs quando o jogador as coleta
    public void IncrementOrbCount()
    {
        orbCount++;
        Debug.Log("Contagem de orbs: " + orbCount);  // Debug para ver o valor quando solicitado
    }

    void DropOrbs()
    {
        // Libera as orbs na posição do inimigo
        ReleaseOrbs(transform.position);
    }
}