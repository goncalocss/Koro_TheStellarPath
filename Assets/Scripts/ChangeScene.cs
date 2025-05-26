using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Nome da cena a carregar")]
    public string nomeCena;

    // Método público que podes chamar por botão, trigger, etc.
    public void MudarCena()
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            SceneManager.LoadScene(nomeCena);
        }
        else
        {
            Debug.LogWarning("Nome da cena não definido no inspetor!");
        }
    }
}
