using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private UnityEvent onInteract;

    private bool isFocused;

    public bool IsInteractable => isInteractable;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 5f;
        outline.enabled = false;
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

        outline.enabled = true;
    }

    public void OnFocusLost()
    {
        if (!isFocused)
            return;

        isFocused = false;

        outline.enabled = false;
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