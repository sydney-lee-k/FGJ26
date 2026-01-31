using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, InputActions.IPlayerActions
{
    private Vector2 inputVector;
    public Vector2 InputVector => inputVector;

    private Vector2 stickDelta;
    public Vector2 StickDelta => stickDelta;

    public bool HasInput => inputVector.sqrMagnitude > 0.01f;

    public bool InteractPressedThisFrame { get; private set; }

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
        InteractPressedThisFrame = false;
    }

    public void ResetValues()
    {
        inputVector = stickDelta = Vector2.zero;
        ClearOneFrameInputFlags();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        stickDelta = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            InteractPressedThisFrame = true;
        }
    }
}