using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialogueCanvas;

    private void Start()
    {
        dialogueCanvas.SetActive(false);
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Enter");
        if (other.CompareTag("Player"))
        {
            dialogueCanvas.SetActive(true);
            TriggerDialogue();
            DialogueManager.isDialogueActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exit");
        if (other.CompareTag("Player"))
        {
            DialogueManager.isDialogueActive = false;
            dialogueCanvas.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Trigger Stay");
        if (other.CompareTag("Player"))
        {
            DialogueManager.isDialogueActive = true;
        }
    }

}
