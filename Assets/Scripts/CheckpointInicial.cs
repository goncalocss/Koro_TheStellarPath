using UnityEngine;

public class CheckpointInicial : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DefinirCheckpoint(transform.position);
            Debug.Log("📍 Checkpoint inicial definido automaticamente.");
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager não encontrado ao definir o checkpoint inicial.");
        }
    }
}
