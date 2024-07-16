using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    [SerializeField] EndScene endscene;

    void Start()
    {
        Time.timeScale = 0;
        FindObjectOfType<PauseButton>().gameObject.SetActive(false);
    }

    private void Update()
    {
        if (DialogueManager.GameOver)
        {
            Wait();
            endscene.Start();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
    }
}
