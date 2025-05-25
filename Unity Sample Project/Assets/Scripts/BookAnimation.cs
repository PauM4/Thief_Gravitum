using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BookAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private Ease ease = Ease.OutSine;

    [Header("Events")]
    [SerializeField] private UnityEvent onAnimationStart;

    [Header("Fade & Scene Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string sceneToLoad;

    private Vector3 initialLocalPosition;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;

        if (fadeImage != null)
        {
            var c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    public void PlayAnimation()
    {
        onAnimationStart?.Invoke();

        Vector3 targetPosition = initialLocalPosition + transform.up * moveDistance;

        transform.DOLocalMove(targetPosition, moveDuration)
                 .SetEase(ease)
                 .OnComplete(() => FadeToBlack());
    }

    private void FadeToBlack()
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
            });
        }
    }
}