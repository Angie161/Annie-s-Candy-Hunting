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

    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;

    void Start()
    {
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        musicSlider.value =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        sfxSlider.value =
            PlayerPrefs.GetFloat("SFXVolume", 1f);

        lastMusicVolume = musicSlider.value;
        lastSFXVolume = sfxSlider.value;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void Update()
    {
        if (AudioManager.Instance == null)
            return;

        bool musicMuted =
            AudioManager.Instance.musicVolume <= 0.01f;

        bool sfxMuted =
            AudioManager.Instance.sfxVolume <= 0.01f;

        musicOnIcon.SetActive(!musicMuted);
        musicOffIcon.SetActive(musicMuted);

        sfxOnIcon.SetActive(!sfxMuted);
        sfxOffIcon.SetActive(sfxMuted);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMusicVolume(value);

        if (value > 0.01f)
            lastMusicVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetSFXVolume(value);

        if (value > 0.01f)
            lastSFXVolume = value;
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance == null)
            return;

        if (AudioManager.Instance.musicVolume > 0.01f)
        {
            musicSlider.value = 0f;
        }
        else
        {
            float volume =
                Mathf.Max(lastMusicVolume, 0.1f);

            musicSlider.value = volume;
        }
    }

    public void ToggleSFX()
    {
        if (AudioManager.Instance == null)
            return;

        if (AudioManager.Instance.sfxVolume > 0.01f)
        {
            sfxSlider.value = 0f;
        }
        else
        {
            float volume =
                Mathf.Max(lastSFXVolume, 0.1f);

            sfxSlider.value = volume;
        }
    }
}