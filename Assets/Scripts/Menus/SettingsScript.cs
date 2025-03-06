using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clipper;
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    float currentVolume;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    
    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
         /*audioMixer.GetFloat("masterVolume", out float haha);
        haha = MathF.Pow(haha,10) *20;*/
         volumeSlider.value = 1f;

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =  resolutions[i] .width+ " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }


        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;

        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, joinedPlayers.isFullscreen,0);
        joinedPlayers.resolution = resolution;
    }
 

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", MathF.Log10(volume)*20);
        currentVolume = volume;

    }
    public void SetFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        joinedPlayers.isFullscreen = Screen.fullScreen;
    }
}
