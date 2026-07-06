using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    public GameObject musicOnIcon;
    public GameObject musicOffIcon;

    public GameObject sfxOnIcon;
    public GameObject sfxOffIcon;

    void Start()
    {
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void Update()
    {
        if (AudioManager.Instance == null)
            return;

        bool musicMuted = AudioManager.Instance.musicVolume <= 0.01f;
        bool sfxMuted = AudioManager.Instance.sfxVolume <= 0.01f;

        musicOnIcon.SetActive(!musicMuted);
        musicOffIcon.SetActive(musicMuted);

        sfxOnIcon.SetActive(!sfxMuted);
        sfxOffIcon.SetActive(sfxMuted);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void ToggleMusic()
    {
        float newValue = AudioManager.Instance.musicVolume > 0 ? 0 : 1;
        musicSlider.value = newValue;
        AudioManager.Instance.SetMusicVolume(newValue);
    }

    public void ToggleSFX()
    {
        float newValue = AudioManager.Instance.sfxVolume > 0 ? 0 : 1;
        sfxSlider.value = newValue;
        AudioManager.Instance.SetSFXVolume(newValue);
    }
}