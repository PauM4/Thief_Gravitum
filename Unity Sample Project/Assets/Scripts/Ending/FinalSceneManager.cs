using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FinalSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private List<Button> optionButtons; 
    [TextArea(2, 5)]
    [SerializeField] private List<string> finalTexts;    
    [SerializeField] private TextMeshProUGUI resultText; 

    [Header("Settings")]
    [TextArea(2, 5)]
    [SerializeField] private string introText;
    [SerializeField] private float introTextDuration = 5f;
    [SerializeField] private float buttonFadeDelay = 0.5f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        
        mainText.alpha = 0;
        resultText.alpha = 0;
        resultText.text = "";
        resultText.gameObject.SetActive(false);

        foreach (var btn in optionButtons)
        {
            var cg = btn.GetComponent<CanvasGroup>();
            if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            btn.interactable = false;
        }

        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        
        mainText.text = "";
        mainText.DOText(introText, introTextDuration, scrambleMode: ScrambleMode.None);
        mainText.DOFade(1f, fadeDuration);

        yield return new WaitForSeconds(introTextDuration + 0.2f);

        
        for (int i = 0; i < optionButtons.Count; i++)
        {
            var cg = optionButtons[i].GetComponent<CanvasGroup>();
            cg.DOFade(1f, fadeDuration);
            optionButtons[i].interactable = true;
            yield return new WaitForSeconds(buttonFadeDelay);
        }

        
        for (int i = 0; i < optionButtons.Count; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    private void OnOptionSelected(int index)
    {
        
        foreach (var btn in optionButtons)
        {
            btn.interactable = false;
            btn.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration);
        }

        
        mainText.DOFade(0f, fadeDuration);

        
        StartCoroutine(ShowFinalText(index));
    }

    private IEnumerator ShowFinalText(int index)
    {
        yield return new WaitForSeconds(fadeDuration + 0.2f);
        resultText.gameObject.SetActive(true);
        resultText.text = "";
        resultText.alpha = 0;
        resultText.DOText(finalTexts[index], introTextDuration, scrambleMode: ScrambleMode.None);
        resultText.DOFade(1f, fadeDuration);
    }
}
