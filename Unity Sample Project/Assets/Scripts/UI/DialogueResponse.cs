using UnityEngine;
using UnityEngine.Events;

public class DialogueResponse : MonoBehaviour
{
    [Header("Texto principal del diálogo")]
    [TextArea(3, 5)]
    public string mainDialogue;

    [Header("Opción 1")]
    public string option1Text;
    public UnityEvent option1Event;

    [Header("Opción 2")]
    public string option2Text;
    public UnityEvent option2Event;

    [Header("Opción 3")]
    public string option3Text;
    public UnityEvent option3Event;

    [Header("Opción 4")]
    public string option4Text;
    public UnityEvent option4Event;

    [Header("Configuración de entrada")]
    public string playerTag = "Player";
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInRange = false;
    private bool yaActivado = false;


    /// <summary>
    /// Llama al DialogueManager para mostrar el diálogo con las opciones configuradas.
    /// </summary>
    public void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("No existe un DialogueManager en la escena.");
            return;
        }

        DialogueManager.Instance.ShowDialogue(
            mainDialogue,
            option1Text, () => option1Event?.Invoke(),
            option2Text, () => option2Event?.Invoke(),
            option3Text, () => option3Event?.Invoke(),
            option4Text, () => option4Event?.Invoke()
        );
        gameObject.SetActive(false);
    }
}
