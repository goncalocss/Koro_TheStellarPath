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
    }

    // Update is called once per frame
    public void MainMenu()
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
