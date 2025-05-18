using UnityEngine;

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
    }
}
