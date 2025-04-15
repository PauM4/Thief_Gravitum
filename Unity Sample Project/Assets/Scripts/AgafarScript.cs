using UnityEngine;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    private Outline outline;

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
        }
    }

    public void Hover()
    {
        if (!Interaction.isExamining && outline != null)
        {
            outline.enabled = true;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 5;
        }
    }

    public void Interact()
    {
        // De moment gestionat des d'Interaction.cs
    }
}
