using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    protected Collider col;

    protected virtual void Awake()
    {

    }

    public abstract void Interact(GameObject interactor);

    public virtual void ShowHighlight(bool show)
    {
        // Optional: add visual feedback, e.g., outline, UI prompt
        // Example: enable/disable a child highlight object
    }
}
