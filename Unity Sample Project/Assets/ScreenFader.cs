using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true); // Activar la imagen
        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(t));
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 1f;
        Color c = fadeImage.color;

        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(t));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // Desactivar al terminar
    }
}
