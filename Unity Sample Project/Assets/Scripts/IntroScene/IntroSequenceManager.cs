using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntroSequenceManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image imageA;
    [SerializeField] private Image imageB;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Narrative Content")]
    [SerializeField] private List<Sprite> backgroundSprites;
    [TextArea(2, 5)]
    [SerializeField] private List<string> dialogueLines;

    [Header("Settings")]
    [SerializeField] private float totalDurationInSeconds = 60f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float maxTextDuration = 8f;
    [SerializeField] private string nextSceneName;

    private bool isUsingA = true;

    private void Start()
    {
        dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0); // Start invisible

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        int steps = Mathf.Max(backgroundSprites.Count, dialogueLines.Count);
        float timePerStep = totalDurationInSeconds / steps;

        for (int i = 0; i < steps; i++)
        {
            float textDuration = Mathf.Min(timePerStep - fadeDuration, maxTextDuration);

            if (i < backgroundSprites.Count)
                StartCoroutine(CrossfadeToSprite(backgroundSprites[i]));

            if (i < dialogueLines.Count)
                StartCoroutine(ShowTextWithFade(dialogueLines[i], textDuration));

            yield return new WaitForSeconds(timePerStep);
        }

        LoadNextScene();
    }

    private IEnumerator CrossfadeToSprite(Sprite newSprite)
    {
        Image fadeOut = isUsingA ? imageA : imageB;
        Image fadeIn = isUsingA ? imageB : imageA;

        fadeIn.sprite = newSprite;
        fadeIn.color = new Color(1, 1, 1, 0);

        Sequence s = DOTween.Sequence();
        s.Join(fadeOut.DOFade(0f, fadeDuration));
        s.Join(fadeIn.DOFade(1f, fadeDuration));
        s.Play();

        yield return s.WaitForCompletion();

        isUsingA = !isUsingA;
    }

    private IEnumerator ShowTextWithFade(string text, float duration)
    {
        yield return dialogueText.DOFade(0f, fadeDuration).WaitForCompletion();

        dialogueText.text = "";
        
        dialogueText.DOText(text, duration, scrambleMode: ScrambleMode.None);
        yield return dialogueText.DOFade(1f, fadeDuration).WaitForCompletion();

        yield return new WaitForSeconds(duration);
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
