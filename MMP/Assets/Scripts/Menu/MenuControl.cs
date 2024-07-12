using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

using UnityEngine.UI;
using TMPro;  
using Util;

public class MenuControl : MonoBehaviour
{
    public GameObject settingsPanel;
    public TMP_Dropdown spaceFunctionDropdown;  
    void Start() 
    {
        settingsPanel.SetActive(false);  
        if (spaceFunctionDropdown != null) {
            spaceFunctionDropdown.onValueChanged.AddListener(delegate { OnDropdownChange(); });
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    } 
  

    public void ToggleSettings()
    {
        settingsPanel.SetActive(true);
    }
    public void closeToggleSettings()
    {
        settingsPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void OnDropdownChange()
    {
        // 0: Jump
        // 1: Portal
        bool useSpaceForJump = spaceFunctionDropdown.value == 0;
        InputUtil.SetUseSpaceForJump(useSpaceForJump);
    }
}