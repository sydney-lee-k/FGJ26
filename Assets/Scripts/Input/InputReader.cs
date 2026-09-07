using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : MonoBehaviour, InputActions.IPlayerActions
{
    public static InputReader instance;
    private InputActions inputActions;
    
    public Vector2 MoveVector { get; private set; }
    public Vector2 LookDelta { get; private set; }
    
    public bool attackJustPressed { get; private set; }
    public bool attackHeld { get; private set; }

    public bool switchJustPressed { get; private set; }
    public bool switchHeld { get; private set; }
    
    public bool interactJustPressed { get; private set; }
    public bool interactHeld { get; private set; }
    
    
    public event Action InteractPressed;
    public event Action SwitchPressed;
    public event Action<bool> AttackInputDown;
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
        attackJustPressed = inputActions.Player.Attack.WasPressedThisFrame();
        attackHeld = inputActions.Player.Attack.IsPressed();
        switchJustPressed = inputActions.Player.Switch.WasPressedThisFrame();
        switchHeld = inputActions.Player.Switch.IsPressed();
        interactJustPressed = inputActions.Player.Interact.WasPressedThisFrame();
        interactHeld = inputActions.Player.Interact.IsPressed();
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) InteractPressed?.Invoke();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.started) SwitchPressed?.Invoke();
    }
}