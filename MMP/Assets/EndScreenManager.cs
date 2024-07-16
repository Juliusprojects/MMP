using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] EndScene endscene;
    private bool ended = false;
    private void Update()
    {
        if (DialogueManager.GameOver)
        {
            if (!ended)
            {
                End();
            }
            ended = true;

        }
    }

    void End()
    {
        Wait();
        endscene.gameObject.SetActive(true);
        endscene.Start();
    }



    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f);
    }
}
