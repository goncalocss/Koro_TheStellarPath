using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;  // Necessário para acessar o TextMesh Pro

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI orbCountText;  // Referência ao TextMesh Pro (UI) para exibir a quantidade de orbs
    private int orbCount = 0;  // Contador de orbs
    public List<Image> pontosDeVida; // Referências às imagens dos pontos de vida na UI
    private int vidaAtual = 6;
    private int acertosRecebidos = 0; // Contador de acertos antes de perder um ponto

    [Header("Referência ao Player")]
    public Player player;

    public bool playerVivo = true;

    // Chama quando uma orb é coletada
    public void IncrementOrbCount()
    {
        orbCount++;  // Incrementa a contagem de orbs
        UpdateOrbCountText();  // Atualiza o texto na UI
    }

    // Atualiza o texto da UI com a contagem atual de orbs
    private void UpdateOrbCountText()
    {
        orbCountText.text = "x" + orbCount;  // Exibe a quantidade de orbs
    }

    // Pode ser chamada para resetar a contagem se o jogador reiniciar
    public void ResetOrbCount()
    {
        orbCount = 0;  // Reseta a contagem de orbs
        UpdateOrbCountText();  // Atualiza o texto na UI
    }

    private void Start()
    {
        // Verifique se o TextMesh Pro foi atribuído no Inspector
        if (orbCountText != null)
        {
            UpdateOrbCountText();  // Exibe o número inicial de orbs (que será 0 inicialmente)
        }


    }

    public void ReceberDano()
    {
        acertosRecebidos++;

        if (acertosRecebidos >= 3)
        {
            PerderPontoDeVida();
            acertosRecebidos = 0;
        }
    }

    void PerderPontoDeVida()
    {
        if (vidaAtual > 0)
        {
            vidaAtual--;
            pontosDeVida[vidaAtual].enabled = false; // Esconde a imagem do ponto de vida
        }

        if (vidaAtual == 0)
        {
            playerVivo = false;
            player.Morrer();
        }
    }
}