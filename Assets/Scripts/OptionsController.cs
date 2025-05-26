using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public Slider volumeSlider;

    private const string FullscreenKey = "Fullscreen";
    private const string VolumeKey = "Volume";

    void Start()
    {
        // Carregar preferências
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        // Aplica fullscreen salvo
        Screen.fullScreen = isFullscreen;

        // Aplica volume salvo e atualiza slider
        AudioListener.volume = volume;
        volumeSlider.value = volume;

        // Listener para o slider (volume muda logo)
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    // Método para ligar no botão Fullscreen
    public void SetFullScreen()
    {
        Screen.fullScreen = true;
        PlayerPrefs.SetInt(FullscreenKey, 1);
        PlayerPrefs.Save();
    }

    // Método para ligar no botão Janela
    public void SetWindowed()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(1280, 720, false); // Ajusta resolução se quiseres
        PlayerPrefs.SetInt(FullscreenKey, 0);
        PlayerPrefs.Save();
    }

    private void OnVolumeSliderChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }
}
