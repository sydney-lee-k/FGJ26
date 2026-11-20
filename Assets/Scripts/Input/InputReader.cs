using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, InputActions.IPlayerActions
{
    public static InputReader instance;
    private InputActions inputActions;
    
    public Vector2 MoveVector { get; private set; }
    public Vector2 LookDelta { get; private set; }
    
    public bool AttackJustPressed { get; private set; }
    public bool AttackHeld { get; private set; }
    public bool AimHeld { get; private set; }
    public bool FocusHeld { get; private set; }
    public bool SwitchJustPressed { get; private set; }
    public bool SwitchHeld { get; private set; }
    
    public bool InteractJustPressed { get; private set; }
    public bool InteractHeld { get; private set; }
    
    
    public event Action InteractPressed;
    public event Action SwitchPressed;
    public event Action DropItemPressed;
    public event Action<bool> AttackInputDown;
    public event Action<bool> AimInputDown;
    public event Action<bool> OnFocusDown;
    
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            inputActions = new InputActions();
            inputActions.Player.SetCallbacks(this);
            inputActions.Enable();
        }
        else
        {
            Debug.Log("Trying to create a second instance of Input Controller on: " + gameObject.name);
        }
    }

    
    public void EnablePlayerInput() => inputActions?.Player.Enable();
    public void DisablePlayerInput() => inputActions?.Player.Disable();
    
    void Update()
    {
        AttackJustPressed = inputActions.Player.Attack.WasPressedThisFrame();
        AttackHeld = inputActions.Player.Attack.IsPressed();
        AimHeld = inputActions.Player.Aim.IsPressed();
        SwitchJustPressed = inputActions.Player.Switch.WasPressedThisFrame();
        SwitchHeld = inputActions.Player.Switch.IsPressed();
        InteractJustPressed = inputActions.Player.Interact.WasPressedThisFrame();
        InteractHeld = inputActions.Player.Interact.IsPressed();
        FocusHeld = inputActions.Player.Focus.IsPressed();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveVector = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookDelta = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) AttackInputDown?.Invoke(true);
        else if (context.canceled) AttackInputDown?.Invoke(false);
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started) AimInputDown?.Invoke(true);
        else if (context.canceled) AimInputDown?.Invoke(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) InteractPressed?.Invoke();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.started) SwitchPressed?.Invoke();
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        if (context.started) OnFocusDown?.Invoke(true);
        else if (context.canceled) OnFocusDown?.Invoke(false);
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (context.started) DropItemPressed?.Invoke();
    }
}