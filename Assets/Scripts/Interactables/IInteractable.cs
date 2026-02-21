using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; }

    void Interact();

    void OnFocusGained();
    void OnFocusLost();
}