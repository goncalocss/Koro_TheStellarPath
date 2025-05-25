using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject fogo; // Referência ao GameObject do fogo (Particle System, etc.)
    private bool ativado = false;

    public Transform pontoDeRespawn; // arrastar no Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            ativado = true;

            if (fogo != null)
            {
                fogo.SetActive(true);
                Debug.Log("🔥 Fogo aceso no checkpoint: " + gameObject.name);
            }

            Vector3 offset = new Vector3(2f, 0f, 0f); // ← deslocamento desejado
            Vector3 posicaoCheckpoint = transform.position + offset;

            GameManager.Instance.DefinirCheckpoint(posicaoCheckpoint);
            Debug.Log("✅ Checkpoint ativado em posição: " + posicaoCheckpoint);

            SoundManager.Instance.PlaySFX("checkpoint");
            SaveData data = new SaveData();
            data.playerPosition = other.transform.position;
            data.playerHealth = GameManager.Instance.ObterVidaAtual();
            data.currentScene = SceneManager.GetActiveScene().name;
            data.checkpointPosition = posicaoCheckpoint; // ← mesma posição deslocada
            data.orbs = GameManager.Instance.ObterOrbs();
            data.vidaMaxima = GameManager.Instance.ObterVidaMaxima();
            data.bananaCount = GameManager.Instance.ObterBananaCount();
            data.caixasDestruidas = GameManager.Instance.ObterCaixasDestruidas();
            data.nivelArma = GameManager.Instance.sistemaArmas.ObterNivelAtual();

            SaveSystem.SaveGame(data);

            Debug.Log("💾 Progresso gravado ao ativar o checkpoint.");
        }
    }

}
