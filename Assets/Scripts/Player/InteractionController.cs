using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;

    [Header("Interaction")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private LayerMask interactableLayer = ~0;

    private readonly Collider[] buffer = new Collider[32];
    private IInteractable currentInteractable;

    private void Awake()
    {
        
    }

    private void Update()
    {
        IInteractable nearest = FindNearestInteractable();
        UpdateFocus(nearest);

        if (currentInteractable != null && inputReader.InteractPressedThisFrame)
        {
            if (currentInteractable.IsInteractable) currentInteractable.Interact();
            AudioManager.PlaySound(SoundType.SHOTGUN);
        }
    }

    private IInteractable FindNearestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, buffer, interactableLayer, QueryTriggerInteraction.Collide);
        IInteractable nearest = null;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider col = buffer[i];
            if (col == null) continue;

            IInteractable interactable = col.GetComponentInParent<IInteractable>();

            if (interactable == null) continue;
            if (!interactable.IsInteractable) continue;

            float distSqr = (col.transform.position - transform.position).sqrMagnitude;
            if (distSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distSqr;
                nearest = interactable;
            }
        }

        return nearest;
    }

    private void UpdateFocus(IInteractable interactable)
    {
        if (ReferenceEquals(currentInteractable, interactable)) return;
        currentInteractable?.OnFocusLost();
        currentInteractable = interactable;
        currentInteractable?.OnFocusGained();
    }
}