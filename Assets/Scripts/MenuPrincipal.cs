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
            GameObject temp = new GameObject("TempSaveData");
            TempSaveData tsd = temp.AddComponent<TempSaveData>();
            tsd.saveData = data;

            SceneManager.LoadScene(data.currentScene); // ← depois disto, OnSceneLoaded vai correr
        }
    }



    public void NovoJogo()
    {
        SaveSystem.DeleteSave(); // Limpa saves antigos se houver
        SceneManager.LoadScene("Verdalya"); // Troca isto pelo nome real da cena inicial
    }
}
