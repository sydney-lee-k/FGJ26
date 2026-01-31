using UnityEngine;
using UnityEngine.Events;

public class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent onInteract;

    private bool isFocused;

    public bool IsInteractable => isInteractable;

    // outline stuff

    private void Awake()
    {
        
    }

    public void Interact()
    {
        if (!isInteractable)
            return;

        Debug.Log($"Interacted with: {gameObject.name}");
        onInteract?.Invoke();
    }

    public void OnFocusGained()
    {
        if (isFocused || !isInteractable)
            return;

        isFocused = true;

        // outline stuff
    }

    public void OnFocusLost()
    {
        if (!isFocused)
            return;

        isFocused = false;

        // outline stuff
    }

    public virtual void SetInteractable(bool value)
    {
        if (isInteractable == value)
            return;

        isInteractable = value;

        if (!isInteractable)
            OnFocusLost();
    }
}