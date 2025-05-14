using UnityEngine;
using TMPro;  // Necessário para acessar o TextMesh Pro

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI orbCountText;  // Referência ao TextMesh Pro (UI) para exibir a quantidade de orbs
    private int orbCount = 0;  // Contador de orbs

    // Chama quando uma orb é coletada
    public void IncrementOrbCount()
    {
        orbCount++;  // Incrementa a contagem de orbs
        UpdateOrbCountText();  // Atualiza o texto na UI
    }

    // Atualiza o texto da UI com a contagem atual de orbs
    private void UpdateOrbCountText()
    {
        orbCountText.text = ":" + orbCount;  // Exibe a quantidade de orbs
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
}