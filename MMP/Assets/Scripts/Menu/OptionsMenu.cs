using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Util;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown spaceFunctionDropdown;
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;
    public Slider masterVolume;
    public Slider musicVolume;

    // Start is called before the first frame update
    void Start()
    { 
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

            resolutionDropDown.AddOptions(options);
            resolutionDropDown.value = currentResolutionIndex;
            resolutionDropDown.RefreshShownValue();
        }
        if (spaceFunctionDropdown != null)
        {
            spaceFunctionDropdown.onValueChanged.AddListener(delegate { OnDropdownChange(); });
        }
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen (bool isfullscreen) 
    {
        Screen.fullScreen = isfullscreen;
    }
    
    public void SetQuality (int qualityindex)
    {
        QualitySettings.SetQualityLevel(qualityindex);
    }

    public void SetMasterVolume (float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetMute (bool isMute)
    {
        if (isMute)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    private void OnDropdownChange()
    {
        // 0: Jump
        // 1: Portal
        bool useSpaceForJump = spaceFunctionDropdown.value == 0;
        InputUtil.SetUseSpaceForJump(useSpaceForJump);
    }
}
