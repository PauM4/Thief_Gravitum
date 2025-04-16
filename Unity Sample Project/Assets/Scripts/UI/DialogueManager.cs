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
    [Tooltip("Panel principal del diálogo (Canvas/Panel que agrupa el texto y los botones).")]
    public GameObject dialoguePanel;

    [Tooltip("Texto principal donde se mostrará el diálogo.")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("Botones de opción, cada uno con su texto hijo. (Hasta 4).")]
    public Button[] optionButtons;

    [Header("Eventos para mostrar/ocultar")]
    [Tooltip("Se invoca justo al mostrar el diálogo (desactivar movimiento del player, mostrar cursor, etc.).")]
    public UnityEvent onDialogueShow;
    [Tooltip("Se invoca justo al ocultar el diálogo (reactivar movimiento, ocultar cursor, etc.).")]
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
    /// Muestra el diálogo con un texto principal y hasta 4 opciones, 
    /// cada opción con su propia acción al pulsar el botón.
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

        // Lanzamos el evento de "mostrar diálogo"
        onDialogueShow?.Invoke();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Reseteamos el texto del diálogo
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

        // Empezamos la animación de escritura con DOTween (ajusta el tiempo a tu gusto)
        float duration = 1.5f;
        dialogueText.DOText(fullDialogueText, duration).OnComplete(() =>
        {
            // Cuando termine de "escribirse" el texto, configuramos las opciones
            // Emparejamos texto de opción y acción en arrays paralelos
            string[] optionTexts = { option1, option2, option3, option4 };
            UnityAction[] optionActions = { action1, action2, action3, action4 };

            for (int i = 0; i < optionTexts.Length; i++)
            {
                if (!string.IsNullOrEmpty(optionTexts[i]) && optionButtons.Length > i)
                {
                    // Activamos el botón
                    optionButtons[i].gameObject.SetActive(true);

                    // Actualizamos el texto del botón
                    TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null)
                        btnText.text = optionTexts[i];

                    // Captura segura del índice para evitar error en la lambda
                    int capturedIndex = i;

                    // Añadimos la acción correspondiente al onClick del botón
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
    /// Cierra u oculta el diálogo, activa el evento onDialogueHide.
    /// </summary>
    public void HideDialogue()
    {
        // Ocultamos el panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Llamamos al evento de "ocultar diálogo"
        onDialogueHide?.Invoke();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Reseteamos texto por si quieres evitar "basura" en la UI
        dialogueText.text = "";
    }
}
