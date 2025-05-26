using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // assign no Inspector
    public string proximaCena;       // nome da cena a carregar depois do vídeo

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer não atribuído!");
            return;
        }

        // Registar callback para saber quando o vídeo terminar
        videoPlayer.loopPointReached += OnVideoFinished;

        // Começar a reproduzir
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Cutscene terminou. Carregando próxima cena: " + proximaCena);

        if (!string.IsNullOrEmpty(proximaCena))
        {
            SceneManager.LoadScene(proximaCena);
        }
        else
        {
            Debug.LogWarning("Próxima cena não foi definida no inspector!");
        }
    }
}
