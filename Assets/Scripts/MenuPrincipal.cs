using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuPrincipal : MonoBehaviour
{
    public Button continuarButton;
    public AudioSource backgroundMusic;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Cutscene")]
    public GameObject cutscenePlayer;           // objeto com StreamingVideoPlayer (desativado inicialmente)
    public StreamingVideoPlayer streamingVideoPlayer;

    public GameObject gameManagerPrefab; // ✅ NOVO: arrasta o prefab no Inspector

    private void Start()
    {
        // Mostra o botão continuar só se houver save
        continuarButton.gameObject.SetActive(SaveSystem.SaveExists());
        SoundManager.Instance.PlayMusic("mainmenu-song");
    }

    public void ContinuarJogo()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            // ✅ Instanciar GameManager se não existir
            if (GameManager.Instance == null)
            {
                GameObject gm = Instantiate(gameManagerPrefab);
                gm.name = "GameManager (Instanciado via Menu)";
                DontDestroyOnLoad(gm);
                Debug.Log("✅ GameManager criado no menu antes de carregar a cena.");
            }

            // Criar TempSaveData com os dados carregados
            GameObject temp = new GameObject("TempSaveData");
            TempSaveData tsd = temp.AddComponent<TempSaveData>();
            tsd.saveData = data;

            Debug.Log("📂 Save carregado, a mudar para cena: " + data.currentScene);
            SceneManager.LoadScene(data.currentScene);
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum save encontrado!");
        }
    }


    public void NovoJogo()
    {
        Debug.Log("🆕 NovoJogo() chamado.");
        StartCoroutine(NovoJogoComFade());
        SoundManager.Instance.StopMusic();
    }

    private IEnumerator NovoJogoComFade()
    {
        SaveSystem.DeleteSave();

        if (GameManager.Instance == null)
        {
            GameObject gm = Instantiate(gameManagerPrefab);
            gm.name = "GameManager (Instanciado via NovoJogo)";
            DontDestroyOnLoad(gm);
            Debug.Log("✅ GameManager criado no Novo Jogo.");
        }

        // Fade In
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }


        cutscenePlayer.SetActive(true);
        streamingVideoPlayer.PlayVideo();
    }
}
