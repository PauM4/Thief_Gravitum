using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    private Outline outline;
    public UnityEvent InteractionEvent;
    public UnityEvent CanvasEvent;
    public UnityEvent DisableCanvasEvent;
    bool hasDoneEvent = false;
    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null && outline.enabled)
        {
            outline.enabled = false;
        }
    }

    public void NoHover()
    {
        if (!Interaction.isExamining && outline != null)
        {
            outline.enabled = false;
            DisableCanvasEvent?.Invoke();
        }
    }

    public void Hover()
    {
        if (!Interaction.isExamining && outline != null)
        {
            outline.enabled = true;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 5;
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartEvent();
            }
            else if(gameObject.tag == "Book")
            {
                StartEvent();
            }

        }

        CanvasEvent?.Invoke();
    }

    public void Interact()
    {
        // De moment gestionat des d'Interaction.cs
    }
    public void StartEvent()
    {
        if (!hasDoneEvent && InteractionEvent != null)
        {
            hasDoneEvent = true;
            InteractionEvent.Invoke();
        }
    }


}
