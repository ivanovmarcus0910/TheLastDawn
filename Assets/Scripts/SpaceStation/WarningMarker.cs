using UnityEngine;
using System.Collections;

public class WarningMarker : MonoBehaviour
{
    public float lifetime = 1f; 
    public float fadeDuration = 0.5f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("WarningMarker: SpriteRenderer not found!");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        Color startColor = spriteRenderer.color;
        startColor.a = 0.7f; 
        spriteRenderer.color = startColor;

        yield return new WaitForSeconds(lifetime - fadeDuration);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Lerp(startColor.a, 0f, timer / fadeDuration);
            spriteRenderer.color = currentColor;
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        Destroy(gameObject); 
    }
}
