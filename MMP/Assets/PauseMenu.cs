using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public AudioMixer audioMixer;
    public Slider musicVolume;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public bool IsActive()
    {
        if (pauseMenu.activeSelf)
        {
            return true;
        }
        else 
        { 
            return false;
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }
}
