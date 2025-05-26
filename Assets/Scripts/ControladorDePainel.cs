using UnityEngine;

public class ControladorDePainel : MonoBehaviour
{
    [Header("Painel a controlar")]
    public GameObject painel;

    public void AbrirPainel()
    {
        if (painel != null)
        {
            painel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Nenhum painel atribuído ao script.");
        }
    }

    public void FecharPainel()
    {
        if (painel != null)
        {
            painel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Nenhum painel atribuído ao script.");
        }
    }
}
