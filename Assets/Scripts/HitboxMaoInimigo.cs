using UnityEngine;

public class HitboxMaoInimigo : MonoBehaviour
{
    public float tempoEntreAtaques = 1f;
    private float tempoUltimoAtaque;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= tempoUltimoAtaque)
        {
            tempoUltimoAtaque = Time.time + tempoEntreAtaques;

            Debug.Log("👊 A MÃO do lagarto acertou o jogador!");

            // Usa a instância global correta do GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReceberDano();
            }
            else
            {
                Debug.LogWarning("⚠️ GameManager.Instance está null!");
            }
        }
    }
}
