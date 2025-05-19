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
        Debug.Log("🔸 HitBox chamado para: " + other.gameObject.name);

        // Tenta obter o identificador (se existir)
        IdentificadorPersistente identificador = other.GetComponent<IdentificadorPersistente>();
        string id = identificador != null ? identificador.idUnico : null;

        if (other.CompareTag("Box"))
        {
            if (!string.IsNullOrEmpty(id))
            {
                GameManager.Instance.RegistarCaixaDestruida(id);
                Debug.Log($"📦 Caixa destruída com ID: {id}");
            }

            ReleaseOrbs(other.transform.position);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Inimigo"))
        {
            Debug.Log("💥 Inimigo atingido e orbs dropadas: " + other.gameObject.name);
            Invoke("DropOrbs", 1.5f); // drop genérico, se quiseres mudar para posição, adapta
        }
        else if (other.CompareTag("Bau"))
        {
            if (!string.IsNullOrEmpty(id) && GameManager.Instance.CaixaJaFoiDestruida(id))
            {
                Debug.Log($"⚠️ Baú {id} já foi ativado anteriormente. Ignorado.");
                return;
            }

            if (!string.IsNullOrEmpty(id))
            {
                GameManager.Instance.RegistarCaixaDestruida(id);
            }

            if (bausAtivados.Contains(other.gameObject)) return;
            bausAtivados.Add(other.gameObject);

            Debug.Log("📤 Baú ativado e banana preparada para drop: " + other.gameObject.name);

            bananaDropPosition = other.transform.position;
            Invoke("DropBanana", 1.5f);

            // ❗️Aqui destróis o script (this), mas não o baú — se quiseres esconder o baú:
            // other.GetComponent<MeshRenderer>().enabled = false;
            // other.GetComponent<Collider>().enabled = false;

            Destroy(gameObject, 3f);
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
