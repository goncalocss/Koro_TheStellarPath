using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class MenuPrincipal : MonoBehaviour
{
    public Button continuarButton;
    public AudioSource backgroundMusic;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Cutscene")]
    public GameObject cutscenePlayer;
    public VideoPlayer videoPlayer;

    private void Start()
    {
        continuarButton.gameObject.SetActive(SaveSystem.SaveExists());
        Debug.Log("🎬 MenuPrincipal iniciado.");
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
        StartCoroutine(NovoJogoComFadeECutscene());
    }

    private IEnumerator NovoJogoComFadeECutscene()
    {
        Debug.Log("🧹 Save antigo eliminado.");
        SaveSystem.DeleteSave();

        // Fade In
        Debug.Log("🎞️ A iniciar fade in...");
        float t = 0f;
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(true);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        Debug.Log("✅ Fade in completo.");

        // Parar música
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            Debug.Log("🔇 Música de fundo parada.");
        }

        // Iniciar cutscene
        Debug.Log("🎥 Ativando cutscenePlayer...");
        cutscenePlayer.SetActive(true);
        videoPlayer.Play();

        // Espera até o vídeo começar (timeout de segurança)
        float timeout = 5f;
        while (!videoPlayer.isPlaying && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (videoPlayer.isPlaying)
        {
            Debug.Log("▶️ Vídeo começou a tocar.");
            fadePanel.gameObject.SetActive(false);
            Debug.Log("🕶️ Painel de fade desativado.");
        }
        else
        {
            Debug.LogWarning("⚠️ O vídeo não começou corretamente.");
        }

        // Esperar o vídeo terminar
        yield return new WaitUntil(() => !videoPlayer.isPlaying);
        Debug.Log("⏹️ Vídeo terminou.");

        // Carregar nova cena
        Debug.Log("🌍 A carregar cena Verdalya...");
        SceneManager.LoadScene("Verdalya");
    }
}
