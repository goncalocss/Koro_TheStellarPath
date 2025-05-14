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
            // Verifica a distância entre o jogador e a caixa
            ReleaseOrbs(other.transform.position);
        }
    }

    // Função para soltar orbs
    private void ReleaseOrbs(Vector3 position)
    {
        int numOrbs = Random.Range(3, 6);
        for (int i = 0; i < numOrbs; i++)
        {
            // Cria as orbs em posições aleatórias próximas
            Vector3 spawnPosition = position + new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f));
            GameObject orb = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);

            // Define a tag da orb instanciada
            orb.tag = "Orb";  // Certifique-se de que a tag "Orb" está definida

            // Adiciona um componente para detectar a colisão com o jogador
            orb.AddComponent<OrbMovement>();


        }
    }

    // Função para puxar as orbs ao jogador tipo imã
    private void PullOrbs()
    {
        // Encontra todas as orbs no jogo
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        foreach (var orb in orbs)
        {
            // Puxa as orbs em direção ao player
            Vector3 direction = (player.position - orb.transform.position).normalized;
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, player.position, 0.1f); // Ajuste de velocidade
        }

    }

    public int GetOrbCount()
    {
        Debug.Log("Contagem de orbs solicitada: " + orbCount);  // Debug para ver o valor quando solicitado
        return orbCount;
    }

    // Método para incrementar o número de orbs quando o jogador as coleta
    public void IncrementOrbCount()
    {
        Debug.Log("Incrementando contagem de orbs. Total atual: " + orbCount);
        orbCount++;
    }
}
