using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // assign no Inspector
    public string proximaCena;       // nome da cena a carregar depois do v�deo

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer n�o atribu�do!");
            return;
        }

        // Registar callback para saber quando o v�deo terminar
        videoPlayer.loopPointReached += OnVideoFinished;

        // Come�ar a reproduzir
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Cutscene terminou. Carregando pr�xima cena: " + proximaCena);

        if (!string.IsNullOrEmpty(proximaCena))
        {
            SceneManager.LoadScene(proximaCena);
        }
        else
        {
            Debug.LogWarning("Pr�xima cena n�o foi definida no inspector!");
        }
    }
}
