using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointInicial : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DefinirCheckpoint(transform.position);
            Debug.Log("📍 Checkpoint inicial definido automaticamente.");

            // ⚠️ Só guarda se ainda não existir save
            if (!SaveSystem.SaveExists())
            {
                SaveData data = new SaveData();
                data.playerPosition = transform.position;
                data.playerHealth = GameManager.Instance.ObterVidaAtual();
                data.currentScene = SceneManager.GetActiveScene().name;
                data.checkpointPosition = transform.position;
                data.orbs = GameManager.Instance.ObterOrbs();

                SaveSystem.SaveGame(data);

                Debug.Log("💾 Save inicial criado no checkpoint inicial.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager não encontrado ao definir o checkpoint inicial.");
        }
    }
}
