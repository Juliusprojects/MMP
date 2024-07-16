using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    public static bool isDialogueActive = false;
    public static bool isDone = false;
    
    public GameObject thankYouDialogueCanvas;
    private bool isThankYouDialogueTriggered = false;

    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {

    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (isDone && !isThankYouDialogueTriggered) return;
        isDialogueActive = true;

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        isDone = true;
        FindObjectOfType<DialogueTrigger>().dialogueCanvas.SetActive(false);
        Debug.Log("End Dialogue");
    }

    public void StartThankYouDialogue()
    {
        if (isThankYouDialogueTriggered) return;
        isThankYouDialogueTriggered = true;
        thankYouDialogueCanvas.SetActive(true);
        dialogueText.text = "Thank you";
        StartCoroutine(FadeOutDialogueText());
    }

    private IEnumerator FadeOutDialogueText()
    {
        float fadeDuration = 1.0f;
        float startAlpha = dialogueText.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = dialogueText.color;
            dialogueText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;
            yield return null;
        }

        dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0);
        thankYouDialogueCanvas.SetActive(false);
    }
}
