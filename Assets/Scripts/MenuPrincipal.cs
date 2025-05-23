using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public Button continuarButton; 


    private void Start()
    {
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

            SceneManager.LoadScene(data.currentScene); 
        }
    }



    public void NovoJogo()
    {
        SaveSystem.DeleteSave(); 
        SceneManager.LoadScene("Verdalya"); 
    }
}
