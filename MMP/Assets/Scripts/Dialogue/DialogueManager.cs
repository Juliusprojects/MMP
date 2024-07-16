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
    private GameObject canvas;

    public static bool GameOver = false;
    void Start()
    {
        sentences = new Queue<string>();
        canvas = FindObjectOfType<DialogueTrigger>().dialogueCanvas;
    }

    void Update()
    {

    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (isDone || GameOver) return;
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
        Debug.Log("DISPLAY NEXT");
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
    if (isDone)
    {
        StartEndDialog();
        return;
    }
    isDone = true;
    FindObjectOfType<DialogueTrigger>().dialogueCanvas.SetActive(false);
    Debug.Log("End Dialogue");
}

public void StartEndDialog()
{
    canvas.SetActive(true);
    StartCoroutine(ShowThankYouAndFade());
}

private IEnumerator ShowThankYouAndFade()
{
    isDialogueActive = true;
    dialogueText.text = "Thank you.. finally im freeEeeEEEe..:)";
    yield return new WaitForSeconds(2f);

    CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
    if (canvasGroup == null)
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    SpriteRenderer[] spriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
    if (spriteRenderers.Length == 0)
    {
        Debug.LogWarning("No SpriteRenderers found in the sibling GameObject and its children.");
    }

    float fadeDuration = 2f; 
    float fadeSpeed = 1f / fadeDuration;
    while (canvasGroup.alpha > 0)
    {
        float fadeAmount = Time.deltaTime * fadeSpeed;
        canvasGroup.alpha -= fadeAmount;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a -= fadeAmount;
            spriteRenderer.color = spriteColor;
        }

        yield return null;
    }

    canvasGroup.alpha = 0;
    foreach (SpriteRenderer spriteRenderer in spriteRenderers)
    {
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 0;
        spriteRenderer.color = spriteColor;
    }
    canvas.SetActive(false);
    GameOver = true;
    //isDialogueActive = false;
    //gameObject.SetActive(false);
}



}
