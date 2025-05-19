using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public Button continuarButton; // Liga isto no Inspector!

    private void Start()
    {
        // Oculta o botão "Continuar" se não houver save
        continuarButton.gameObject.SetActive(SaveSystem.SaveExists());
    }

    public void ContinuarJogo()
    {
        Debug.Log("🔁 ContinuarJogo() chamado.");

        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            Debug.Log("📂 Save carregado:");
            Debug.Log($"   Cena: {data.currentScene}");
            Debug.Log($"   Posição: {data.playerPosition}");
            Debug.Log($"   Vida: {data.playerHealth}");
            Debug.Log($"   Checkpoint: {data.checkpointPosition}");
            Debug.Log($"   Orbs: {data.orbs}");

            GameObject temp = new GameObject("TempSaveData");
            TempSaveData tsd = temp.AddComponent<TempSaveData>();
            tsd.saveData = data;

            SceneManager.LoadScene(data.currentScene);
        }
        else
        {
            Debug.LogWarning("❌ SaveData é null.");
        }
    }


    public void NovoJogo()
    {
        SaveSystem.DeleteSave(); // Limpa saves antigos se houver
        SceneManager.LoadScene("NomeDaTuaPrimeiraCena"); // Troca isto pelo nome real da cena inicial
    }
}
