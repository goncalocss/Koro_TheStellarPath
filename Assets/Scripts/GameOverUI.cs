using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Nome da cena inicial (definir no Inspector)")]
    public string nomeCenaInicial;

    // falta aqui o botão de pausar jogo(esc) não funcionar quando o game over estiver ativo

    public void VoltarAoCheckpoint()
    {
        SceneManager.UnloadSceneAsync("GameOver");
        GameManager.Instance.Respawn();
    }

    public void ReiniciarJogo()
    {
        if (!string.IsNullOrEmpty(nomeCenaInicial))
        {
            SceneManager.LoadScene(nomeCenaInicial);
        }
        else
        {
            Debug.LogWarning("❗ Nome da cena inicial não foi definido no Inspector!");
        }
    }
}
