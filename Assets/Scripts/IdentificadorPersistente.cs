using UnityEngine;

public class IdentificadorPersistente : MonoBehaviour
{
    public string idUnico;

    private void Awake()
    {
        if (string.IsNullOrEmpty(idUnico))
        {
            Debug.LogWarning($"❗ Objeto '{name}' não tem idUnico atribuído. Vai falhar no sistema de persistência!");
        }
    }
}
