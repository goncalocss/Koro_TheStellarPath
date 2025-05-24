using UnityEngine;
using System.Collections.Generic;

public class CollectiblesManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject orbPrefab;
    public GameObject bananaPrefab;
    public Transform player;

    private readonly HashSet<GameObject> bausAtivados = new();
    private Vector3 bananaDropPosition;

    public void HitBox(Collider other)
    {
        string id = other.GetComponent<IdentificadorPersistente>()?.idUnico;

        switch (other.tag)
        {
            case "Box":
                if (!string.IsNullOrEmpty(id))
                    GameManager.Instance.RegistarCaixaDestruida(id);

                DropOrbsAt(other.transform.position);
                Destroy(other.gameObject);
                break;

            case "Inimigo":
                Invoke(nameof(DropOrbsAtSelf), 1.5f);
                break;

            case "Bau":
                if (!string.IsNullOrEmpty(id) && GameManager.Instance.CaixaJaFoiDestruida(id))
                    return;

                if (!string.IsNullOrEmpty(id))
                    GameManager.Instance.RegistarCaixaDestruida(id);

                if (bausAtivados.Contains(other.gameObject)) return;
                bausAtivados.Add(other.gameObject);

                bananaDropPosition = other.transform.position;
                Invoke(nameof(DropBananaAtStoredPosition), 1.5f);
                break;
        }
    }

    private void DropOrbsAt(Vector3 position)
    {
        int count = Random.Range(3, 6);
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

            if (!orb.TryGetComponent(out CollectiblesMovement cm))
                cm = orb.AddComponent<CollectiblesMovement>();

            cm.tipo = CollectiblesMovement.TipoColetavel.Orb;
            cm.verticalOffset = 1f;
            cm.moveDuration = 1.5f;
        }
    }

    private void DropOrbsAtSelf()
    {
        DropOrbsAt(transform.position);
    }

    private void DropBananaAtStoredPosition()
    {
        Vector3 spawnPos = bananaDropPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        GameObject banana = Instantiate(bananaPrefab, spawnPos, Quaternion.identity);

        if (!banana.TryGetComponent(out CollectiblesMovement cm))
            cm = banana.AddComponent<CollectiblesMovement>();

        cm.tipo = CollectiblesMovement.TipoColetavel.Banana;
        cm.verticalOffset = 1f;
        cm.moveDuration = 1.5f;
    }
}
