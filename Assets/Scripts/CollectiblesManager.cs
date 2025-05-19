using UnityEngine;
using System.Collections.Generic;

public class CollectiblesManager : MonoBehaviour
{
    public GameObject orbPrefab; // Prefab da orb
    public GameObject bananaPrefab; // Prefab da banana
    public Transform player; // Referência ao player

    public float interactionDistance = 3f;
    private int orbCount = 0;

    private HashSet<GameObject> bausAtivados = new HashSet<GameObject>();
    private Vector3 bananaDropPosition; // posição temporária para DropBanana

    private void Update()
    {
        PullOrbs();
    }

    public void HitBox(Collider other)
    {
        Debug.Log("HitBox chamado para: " + other.gameObject.name);

        if (other.CompareTag("Box"))
        {
            Destroy(other.gameObject);
            ReleaseOrbs(other.transform.position);
        }
        else if (other.CompareTag("Inimigo"))
        {
            Debug.Log("Inimigo atingido e orbs dropadas: " + other.gameObject.name);
            Invoke("DropOrbs", 1.5f);
        }
        else if (other.CompareTag("Bau"))
        {
            if (bausAtivados.Contains(other.gameObject)) return;

            bausAtivados.Add(other.gameObject);

            Debug.Log("📦 Baú atingido: " + other.gameObject.name);

            bananaDropPosition = other.transform.position; // armazena a posição
            Invoke("DropBanana", 1.5f);
            Destroy(gameObject, 3f);
            //Tratar da questão de animação do baú para dropar a banana, ou seja posso desativar o mesh renderer ou o collider
           
        }
    }

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
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        foreach (var orb in orbs)
        {
            if (!orb.GetComponent<OrbMovement>().isCollected)
            {
                Vector3 direction = (player.position - orb.transform.position).normalized;
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, player.position, 0.1f);
            }
        }
    }

    public void IncrementOrbCount()
    {
        orbCount++;
        Debug.Log("Contagem de orbs: " + orbCount);
    }

    void DropOrbs()
    {
        ReleaseOrbs(transform.position);
    }

    // 🍌 BANANAS
    private void ReleaseBananas(Vector3 position)
    {
        Debug.Log("🍌 Banana está a ser instanciada!");

        int numBananas = 1;
        for (int i = 0; i < numBananas; i++)
        {
            Vector3 spawnPosition = position + new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
            GameObject banana = Instantiate(bananaPrefab, spawnPosition, Quaternion.identity);

            banana.tag = "Banana";
            banana.AddComponent<BananaMovement>();
        }
    }


    void DropBanana()
    {
        Debug.Log("🍌 Banana está a ser dropada!");
        ReleaseBananas(bananaDropPosition);
    }
}
