using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuPrincipal : MonoBehaviour
{
    public Button continuarButton;
 

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Cutscene")]
    public GameObject cutscenePlayer;           // objeto com StreamingVideoPlayer (desativado inicialmente)
    public StreamingVideoPlayer streamingVideoPlayer;

    private void Start()
    {
        continuarButton.gameObject.SetActive(SaveSystem.SaveExists());
        SoundManager.Instance.PlayMusic("mainmenu-song");
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

      

        // Ativar e tocar cutscene
        cutscenePlayer.SetActive(true);
        streamingVideoPlayer.PlayVideo();

        // Não espera aqui pelo vídeo, o streamingVideoPlayer vai cuidar disso e trocar a cena sozinho
    }
}
