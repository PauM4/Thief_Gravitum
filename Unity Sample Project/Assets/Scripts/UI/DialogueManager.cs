using UnityEngine;
using UnityEngine.UI;
using TMPro;                 
using UnityEngine.Events;    
using DG.Tweening;           

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private Interaction interaction;

    [Header("Referencias de UI")]
    [Tooltip("Panel principal del di�logo (Canvas/Panel que agrupa el texto y los botones).")]
    public GameObject dialoguePanel;

    [Tooltip("Texto principal donde se mostrar� el di�logo.")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("Botones de opci�n, cada uno con su texto hijo. (Hasta 4).")]
    public Button[] optionButtons;

    [Header("Eventos para mostrar/ocultar")]
    [Tooltip("Se invoca justo al mostrar el di�logo (desactivar movimiento del player, mostrar cursor, etc.).")]
    public UnityEvent onDialogueShow;
    [Tooltip("Se invoca justo al ocultar el di�logo (reactivar movimiento, ocultar cursor, etc.).")]
    public UnityEvent onDialogueHide;

    private string fullDialogueText;

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

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Muestra el di�logo con un texto principal y hasta 4 opciones, 
    /// cada opci�n con su propia acci�n al pulsar el bot�n.
    /// </summary>
    public void ShowDialogue(
        string mainText,
        string option1 = null, UnityEngine.Events.UnityAction action1 = null,
        string option2 = null, UnityEngine.Events.UnityAction action2 = null,
        string option3 = null, UnityEngine.Events.UnityAction action3 = null,
        string option4 = null, UnityEngine.Events.UnityAction action4 = null)
    {
        // Guardamos el texto completo
        fullDialogueText = mainText;

        // Mostramos el panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        interaction.isDialogue = true;

        // Lanzamos el evento de "mostrar di�logo"
        onDialogueShow?.Invoke();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Reseteamos el texto del di�logo
        dialogueText.text = "";

        // Desactivamos todos los botones antes de usarlos
        foreach (Button btn in optionButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(false);
        }

        // Desvinculamos cualquier listener previo de los botones para evitar acumulaciones
        foreach (Button btn in optionButtons)
        {
            if (btn != null)
                btn.onClick.RemoveAllListeners();
        }

        // Empezamos la animaci�n de escritura con DOTween (ajusta el tiempo a tu gusto)
        float duration = 1.5f;
        dialogueText.DOText(fullDialogueText, duration).OnComplete(() =>
        {
            // Cuando termine de "escribirse" el texto, configuramos las opciones
            // Emparejamos texto de opci�n y acci�n en arrays paralelos
            string[] optionTexts = { option1, option2, option3, option4 };
            UnityAction[] optionActions = { action1, action2, action3, action4 };

            for (int i = 0; i < optionTexts.Length; i++)
            {
                if (!string.IsNullOrEmpty(optionTexts[i]) && optionButtons.Length > i)
                {
                    // Activamos el bot�n
                    optionButtons[i].gameObject.SetActive(true);

                    // Actualizamos el texto del bot�n
                    TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null)
                        btnText.text = optionTexts[i];

                    // Captura segura del �ndice para evitar error en la lambda
                    int capturedIndex = i;

                    // A�adimos la acci�n correspondiente al onClick del bot�n
                    optionButtons[i].onClick.AddListener(() =>
                    {
                        HideDialogue();
                        interaction.isDialogue = false;
                        optionActions[capturedIndex]?.Invoke();
                    });
                }
            }

        });
    }

    /// <summary>
    /// Cierra u oculta el di�logo, activa el evento onDialogueHide.
    /// </summary>
    public void HideDialogue()
    {
        // Ocultamos el panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Llamamos al evento de "ocultar di�logo"
        onDialogueHide?.Invoke();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Reseteamos texto por si quieres evitar "basura" en la UI
        dialogueText.text = "";
    }
}
