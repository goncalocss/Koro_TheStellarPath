using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public class StreamingVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public string videoFileName;     // nome do arquivo dentro de StreamingAssets
    public bool looping = true;
    public string nextSceneName;

    public UnityEvent<StreamingVideoPlayer> onVideoFinished;

    public void PlayVideo()
    {
        StartCoroutine(PlayStreamingClip(videoFileName, looping));
    }

    private IEnumerator PlayStreamingClip(string videoFile, bool looping)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile + ".mp4");
        videoPlayer.isLooping = looping;

        var audioSource = videoPlayer.GetComponent<AudioSource>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();
        videoImage.texture = videoPlayer.texture;

        while (videoPlayer.isPlaying)
            yield return null;

        onVideoFinished.Invoke(this);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene not set!");
        }
    }
}
