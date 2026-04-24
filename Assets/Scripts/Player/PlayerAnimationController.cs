using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [Header("References")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float damping = 0.1f;

    private Transform cachedTransform;

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        Vector3 worldMove = movementController.WorldMoveDirection;

        if (worldMove.sqrMagnitude < 0.01f)
        {
            animator.SetFloat("Forward", 0f, damping, Time.deltaTime);
            animator.SetFloat("Right", 0f, damping, Time.deltaTime);
            return;
        }

        Vector3 localMove = cachedTransform.InverseTransformDirection(worldMove);

        float moveForward = Mathf.Clamp(localMove.z, -1f, 1f);
        float moveRight = Mathf.Clamp(localMove.x, -1f, 1f);

        Vector2 animVec = new(moveRight, moveForward);
        animVec = Vector2.ClampMagnitude(animVec, 1f);

        animator.SetFloat("Forward", animVec.y, damping, Time.deltaTime);
        animator.SetFloat("Right", animVec.x, damping, Time.deltaTime);
    }
}
