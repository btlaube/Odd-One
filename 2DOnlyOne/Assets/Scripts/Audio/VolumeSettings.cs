using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioHandler))]
public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private AudioHandler audioHandler;
    private ICanvasAnimator animator;

    void Awake()
    {
        audioHandler = GetComponent<AudioHandler>();
        animator = GetComponent<ICanvasAnimator>();
    }

    void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0f);
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dB = SliderToDecibels(sliderValue);
        audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = SliderToDecibels(sliderValue);
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    private float SliderToDecibels(float sliderValue)
    {
        return sliderValue > 0f ? Mathf.Log10(sliderValue) * 20f : -80f;
    }

    public void ToggleSettingsMenu()
    {
        animator?.Toggle();
    }

}
