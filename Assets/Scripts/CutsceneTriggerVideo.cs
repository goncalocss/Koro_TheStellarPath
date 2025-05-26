using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutsceneTriggerVideo : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;          // RawImage para mostrar o vídeo
    public string videoFileName;         // Nome do arquivo na StreamingAssets (ex: "cutscene1.mp4")

    [Header("UI")]
    public GameObject hudUI;             // HUD para esconder durante cutscene
    public Image fadePanel;              // Painel preto para fade

    [Header("Config")]
    public float fadeDuration = 1.5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
            return;

        hasTriggered = true;

        if (hudUI != null)
            hudUI.SetActive(false);

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Fade In
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Ativar RawImage e configurar VideoPlayer para URL + Render Texture
        videoImage.gameObject.SetActive(true);

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName + ".mp4");
        videoPath = videoPath.Replace("\\", "/");

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.isLooping = false;

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // Linkar Render Texture na RawImage
        videoImage.texture = videoPlayer.texture;

        // Pausar jogo
        Time.timeScale = 0f;

        videoPlayer.Play();

        // Espera o vídeo terminar
        bool videoFinished = false;
        videoPlayer.loopPointReached += vp => videoFinished = true;

        while (!videoFinished)
            yield return null;

        // Fade Out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Restaurar estado do jogo
        Time.timeScale = 1f;

        if (hudUI != null)
            hudUI.SetActive(true);

        videoImage.gameObject.SetActive(false);  
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);

        if (endAlpha == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}
