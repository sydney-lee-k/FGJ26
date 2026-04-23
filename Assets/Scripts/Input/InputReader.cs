using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, InputActions.IPlayerActions
{
    private InputActions controls;

    public Vector2 InputVector { get; private set; }
    public Vector2 StickDelta { get; private set; }

    public event Action InteractPressed;
    //public event Action OnSwitch;
    //public event Action<bool> OnAttack;

    public void Initialize()
    {
        if (controls != null) return;

        controls = new InputActions();
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    public void EnablePlayerInput() => controls?.Player.Enable();
    public void DisablePlayerInput() => controls?.Player.Disable();

    public void OnMove(InputAction.CallbackContext context)
    {
        InputVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        StickDelta = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            InteractPressed?.Invoke();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        /*
        if (context.started)
            OnSwitch?.Invoke();*/
    }
}