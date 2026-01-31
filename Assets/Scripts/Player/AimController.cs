using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private LayerMask groundMask;

    [Header("Gamepad Aim Settings")]
    [SerializeField] private float stickDeadZone = 0.15f;
    [SerializeField] private float aimSmoothing = 12f;

    private Camera mainCamera;

    // Persistent aim direction for stick aiming
    private Vector3 stickAimDirection = Vector3.forward;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("AimController: No Main Camera found.");
            enabled = false;
        }
    }

    private void Update()
    {
        Vector2 stickInput = inputReader.StickDelta;

        // Controller / stick aim takes priority when active
        if (stickInput.sqrMagnitude > stickDeadZone * stickDeadZone)
        {
            UpdateStickAim(stickInput);
        }
        else
        {
            UpdateMouseAim();
        }
    }

    #region Mouse Aim

    private void UpdateMouseAim()
    {
        if (Mouse.current == null)
            return;

        if (!TryGetMouseAimPoint(out Vector3 aimPoint))
            return;

        Vector3 direction = aimPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        transform.forward = direction.normalized;

        // Sync stick direction so switching feels seamless
        stickAimDirection = transform.forward;
    }

    private bool TryGetMouseAimPoint(out Vector3 point)
    {
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        Plane aimPlane = new(
            Vector3.up,
            new Vector3(0f, transform.position.y, 0f)
        );

        if (aimPlane.Raycast(ray, out float enter))
        {
            point = ray.GetPoint(enter);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    #endregion

    #region Stick Aim

    private void UpdateStickAim(Vector2 stickInput)
    {
        Vector3 inputDirection = new(stickInput.x, 0f, stickInput.y);

        if (inputDirection.sqrMagnitude < 0.0001f)
            return;

        inputDirection.Normalize();

        stickAimDirection = Vector3.Slerp(
            stickAimDirection,
            inputDirection,
            aimSmoothing * Time.deltaTime
        );

        transform.forward = stickAimDirection;
    }

    #endregion
}