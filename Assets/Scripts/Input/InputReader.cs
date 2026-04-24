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
    public event Action<bool> AttackInputChanged;

    public void Initialize()
    {
        if (controls != null) return;

        controls = new InputActions();
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    public void EnablePlayerInput() => controls?.Player.Enable();
    public void DisablePlayerInput() => controls?.Player.Disable();

    public void OnMove(InputAction.CallbackContext ctx)
    {
        InputVector = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        StickDelta = ctx.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            AttackInputChanged?.Invoke(true);
        else if (ctx.canceled)
            AttackInputChanged?.Invoke(false);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InteractPressed?.Invoke();
    }

    public void OnSwitch(InputAction.CallbackContext ctx)
    {
        /*
        if (ctx.started)
            OnSwitch?.Invoke();*/
    }
}