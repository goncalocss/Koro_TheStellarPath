using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public class cutsceneAutoPlay : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public string videoFileName;     
    public bool looping = false;     
    public string nextSceneName;     

    public UnityEvent<cutsceneAutoPlay> onVideoFinished;

    private void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        if (videoImage != null)
            videoImage.gameObject.SetActive(true);  

        StartCoroutine(PlayStreamingClip(videoFileName, looping));
    }

    private IEnumerator PlayStreamingClip(string videoFile, bool looping)
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFile + ".mp4");
        videoPath = videoPath.Replace("\\", "/"); 

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
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
