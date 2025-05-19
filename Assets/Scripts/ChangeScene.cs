using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    //Quero uma funçao para mudar de cena onde o parametro é o nome da cena
    public void ChangeSceneByName(string sceneName)
    {
        // Verifica se o nome da cena não é nulo ou vazio
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Muda para a cena especificada
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Nome da cena inválido.");
        }
    }
}
