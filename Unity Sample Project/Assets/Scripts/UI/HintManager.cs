using UnityEngine;
using TMPro;
using DG.Tweening;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    [Header("UI de pista")]
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;

    [Tooltip("Duración del efecto de tipeo con DOTween")]
    public float typingDuration = 1f;

    private Tween currentTween;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    public void ShowHint(string text)
    {
        if (hintPanel == null || hintText == null)
        {
            Debug.LogWarning("HintManager: No se han asignado correctamente los elementos de UI.");
            return;
        }

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        hintPanel.SetActive(true);

        hintText.text = "";

        currentTween = hintText.DOText(text, typingDuration).SetEase(Ease.Linear);
    }

    public void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        hintText.text = "";
    }
}
