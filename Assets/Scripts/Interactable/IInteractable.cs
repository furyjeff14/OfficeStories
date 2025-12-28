using UnityEngine;

public interface IInteractable
{
    // Called when the player interacts with the object
    void Interact(GameObject interactor);

    // Optional: show highlight/UI hint
    void ShowHighlight(bool show);
}