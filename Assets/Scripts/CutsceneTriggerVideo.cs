using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutsceneTriggerVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject hudUI;
    public Image fadePanel; // ← arrastar Image do painel preto

    private bool jaAtivou = false;

    private void OnTriggerEnter(Collider other)
    {
        if (jaAtivou || !other.CompareTag("Player")) return;

        jaAtivou = true;

        if (hudUI != null)
            hudUI.SetActive(false);

        StartCoroutine(FazerFadeInECutscene());
    }

    private IEnumerator FazerFadeInECutscene()
    {
        // Ativar painel (caso esteja desativado)
        fadePanel.gameObject.SetActive(true);

        float duracao = 1.5f;
        float tempo = 0f;
        Color cor = fadePanel.color;

        // Fade in (0 → 1)
        while (tempo < duracao)
        {
            float alpha = Mathf.Lerp(0f, 1f, tempo / duracao);
            fadePanel.color = new Color(cor.r, cor.g, cor.b, alpha);
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }

        fadePanel.color = new Color(cor.r, cor.g, cor.b, 1f);

        // 👇 Remover o painel antes do vídeo começar
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.gameObject.SetActive(false);

        // 🕹️ Pausar jogo
        Time.timeScale = 0f;

        // ▶️ Iniciar vídeo
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();

        videoPlayer.loopPointReached += QuandoCutsceneAcabar;
    }

    private void QuandoCutsceneAcabar(VideoPlayer vp)
    {
        Debug.Log("✅ Cutscene terminada.");

        // Começar fade out antes de restaurar o jogo
        StartCoroutine(FazerFadeOutAntesDeTerminar());
    }

    private IEnumerator FazerFadeOutAntesDeTerminar()
    {
        fadePanel.gameObject.SetActive(true);
        Color cor = fadePanel.color;
        float duracao = 1.5f;
        float tempo = 0f;

        // Fade out (0 → 1)
        while (tempo < duracao)
        {
            float alpha = Mathf.Lerp(0f, 1f, tempo / duracao);
            fadePanel.color = new Color(cor.r, cor.g, cor.b, alpha);
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }

        fadePanel.color = new Color(cor.r, cor.g, cor.b, 1f);

        // 🕹️ Retomar jogo
        Time.timeScale = 1f;

        if (hudUI != null)
            hudUI.SetActive(true);

        videoPlayer.gameObject.SetActive(false);

        // Desativar painel de fade no fim
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
