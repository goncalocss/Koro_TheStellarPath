using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public string nomeCenaInicial;

    public void VoltarAoCheckpoint()
    {
        SceneManager.UnloadSceneAsync("MenuPausa");
        GameManager.Instance.Respawn();
        Time.timeScale = 1f; // DESPAUSA O JOGO

        string cenaAtual = SceneManager.GetActiveScene().name;
        if (cenaAtual == "Verdalya")
        {
            SoundManager.Instance.PlayMusic("World1");
        }
        else if (cenaAtual == "Drexan")
        {
            SoundManager.Instance.PlayMusic("World2");
        }
        else if (cenaAtual == "Nebelya")
        {
            SoundManager.Instance.PlayMusic("World3");
        }
        else
        {
            Debug.LogWarning("❗ Música não definida para a cena atual: " + cenaAtual);
        }
    }

    public void MainMenu()
    {
        if (!string.IsNullOrEmpty(nomeCenaInicial))
        {
            Time.timeScale = 1f; // ✅ Garante que o tempo é restaurado
            SoundManager.Instance.PlayMusic("mainmenu-song");

            // ✅ Destrói GameManager para que seja recriado corretamente no menu
            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
            }

            SceneManager.LoadScene(nomeCenaInicial);
        }
        else
        {
            Debug.LogWarning("❗ Nome da cena inicial não foi definido no Inspector!");
        }
    }
}
