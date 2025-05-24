using UnityEngine;

public class HitboxMaoInimigo : MonoBehaviour
{
    public GameManager gameManager;
    public float tempoEntreAtaques = 1f;
    private float tempoUltimoAtaque;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= tempoUltimoAtaque)
        {
            tempoUltimoAtaque = Time.time + tempoEntreAtaques;

            Debug.Log("A MÃO do lagarto acertou o jogador!");
            gameManager.ReceberDano();
        }
    }

}