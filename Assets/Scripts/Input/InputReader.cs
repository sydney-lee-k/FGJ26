using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, InputActions.IPlayerActions
{
    #region Player Input States

    private Vector2 inputVector;
    public Vector2 InputVector => inputVector;

    private Vector2 mouseDelta;
    public Vector2 MouseDelta => mouseDelta;

    public bool HasInput => inputVector.sqrMagnitude > 0.01f;

    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpReleasedThisFrame { get; private set; }

    public bool RunPressedThisFrame { get; private set; }
    public bool RunReleasedThisFrame { get; private set; }
    public bool RunHeld { get; private set; }

    public bool CrouchPressedThisFrame { get; private set; }
    public bool CrouchReleasedThisFrame { get; private set; }
    public bool CrouchHeld { get; private set; }

    public bool InteractPressedThisFrame { get; private set; }
    public bool OpenCameraPressedThisFrame { get; private set; }

    [Header("Player Events")]
    public UnityEvent JumpEvent = new();
    public UnityEvent RunEvent = new();
    public UnityEvent CrouchEvent = new();
    public UnityEvent InteractEvent = new();
    public UnityEvent CameraOpenedEvent = new();

    [Header("Toggle Settings")]
    public bool HoldToRun = true;     // true = hold to sprint, false = toggle
    public bool HoldToCrouch = true;  // true = hold to crouch, false = toggle

    #endregion

    #region Camera Input States

    public bool TakePhotoPressedThisFrame { get; private set; }
    private float zoomDelta;
    public float ZoomDelta => zoomDelta;

    [Header("Camera Events")]
    public UnityEvent PhotoTakenEvent = new();

    #endregion

    private InputActions controls;

    public void Initialize()
    {
        if (controls != null) return;

        controls = new InputActions();
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    public void EnablePlayerInput() => controls?.Player.Enable();
    public void DisablePlayerInput() => controls?.Player.Disable();

    public void ClearOneFrameInputFlags()
    {
        JumpPressedThisFrame = JumpReleasedThisFrame = false;
        RunPressedThisFrame = RunReleasedThisFrame = false;
        CrouchPressedThisFrame = CrouchReleasedThisFrame = false;
        InteractPressedThisFrame = false;
        OpenCameraPressedThisFrame = false;
        TakePhotoPressedThisFrame = false;
        zoomDelta = 0f;
    }

    public void ResetValues()
    {
        inputVector = mouseDelta = Vector2.zero;
        RunHeld = CrouchHeld = false;
        ClearOneFrameInputFlags();
    }

    #region Player Callbacks

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpPressedThisFrame = true;
            JumpEvent?.Invoke();
        }
        else if (context.canceled)
        {
            JumpReleasedThisFrame = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InteractPressedThisFrame = true;
            InteractEvent?.Invoke();
        }
    }

    #endregion
}