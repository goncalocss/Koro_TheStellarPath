using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointInicial : MonoBehaviour
{
    private bool checkpointDefinido = false;

    private void Start()
    {
        // ✅ Só define se não estamos a carregar de um save existente
        if (TempSaveData.Instance == null)
        {
            TentarDefinirCheckpoint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TentarDefinirCheckpoint();
        }
    }

    private void TentarDefinirCheckpoint()
    {
        if (checkpointDefinido) return; // evita duplicações

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DefinirCheckpoint(transform.position);
            Debug.Log("📍 Checkpoint inicial definido.");

            if (!SaveSystem.SaveExists())
            {
                SaveData data = new SaveData();
                data.playerPosition = transform.position;
                data.playerHealth = GameManager.Instance.ObterVidaAtual();
                data.currentScene = SceneManager.GetActiveScene().name;
                data.checkpointPosition = transform.position;
                data.orbs = GameManager.Instance.ObterOrbs();
                data.vidaMaxima = GameManager.Instance.ObterVidaMaxima();
                data.bananaCount = GameManager.Instance.ObterBananaCount();
                data.caixasDestruidas = GameManager.Instance.ObterCaixasDestruidas();
                data.nivelArma = GameManager.Instance.sistemaArmas.ObterNivelAtual();

                SaveSystem.SaveGame(data);

                Debug.Log("💾 Save criado ao passar no checkpoint inicial.");
            }

            checkpointDefinido = true;
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager não encontrado ao definir o checkpoint inicial.");
        }
    }
}
