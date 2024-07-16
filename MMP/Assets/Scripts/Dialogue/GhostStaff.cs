using UnityEngine;

public class GhostStaff : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private DialogueManager dialogueManager;
    public float fadeDuration = 1.0f;
    public static bool isCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueManager = GetComponentInParent<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            StartCoroutine(FadeOutSprite());
        }
    }

    private IEnumerator FadeOutSprite()
    {
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = spriteRenderer.color;
            spriteRenderer.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        gameObject.SetActive(false);
    }
}
