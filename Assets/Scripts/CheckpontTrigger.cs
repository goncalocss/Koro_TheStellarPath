using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject fogo; // Referência ao GameObject do fogo (Particle System, etc.)
    private bool ativado = false;
    private bool vidaCurada = false;

    private void OnTriggerEnter(Collider other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            ativado = true;

            if (fogo != null)
            {
                fogo.SetActive(true); // Acende o fogo visualmente
                Debug.Log("🔥 Fogo aceso no checkpoint: " + gameObject.name);
            }

            GameManager.Instance.DefinirCheckpoint(transform.position);
            Debug.Log("✅ Checkpoint ativado em posição: " + transform.position);

            // ✅ Cura uma vez, se a vida estiver incompleta
            if (!vidaCurada &&
                GameManager.Instance.ObterVidaAtual() < GameManager.Instance.ObterVidaMaxima())
            {
                GameManager.Instance.ReporVida();
                vidaCurada = true;
                Debug.Log("❤️ Vida restaurada ao ativar o checkpoint.");
            }
        }


        SaveData data = new SaveData();
        data.playerPosition = transform.position;
        data.playerHealth = GameManager.Instance.ObterVidaAtual();
        data.currentScene = SceneManager.GetActiveScene().name;
        data.checkpointPosition = transform.position; // Adiciona a posição do checkpoint
        data.orbs = GameManager.Instance.ObterOrbs(); // Adiciona o número de orbs
        data.vidaMaxima = GameManager.Instance.ObterVidaMaxima(); // Adiciona a vida máxima
        data.bananaCount = GameManager.Instance.ObterBananaCount(); // Adiciona o número de bananas

        SaveSystem.SaveGame(data);

        Debug.Log("💾 Progresso gravado ao ativar o checkpoint.");
    }
}
