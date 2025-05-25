using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Criar e configurar os AudioSources
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string musicName, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/Music/{musicName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] Música '{musicName}' não encontrada!");
            return;
        }

        // ⛔️ Evita reiniciar se a mesma música já estiver a tocar
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/SFX/{sfxName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] SFX '{sfxName}' não encontrado!");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayUISound(string uiSoundName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/UI/{uiSoundName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] UI sound '{uiSoundName}' não encontrado!");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
