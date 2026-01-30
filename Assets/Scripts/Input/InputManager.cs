using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    [Header("Player Input Events")]
    public UnityEvent OnJumpPressed;
    public UnityEvent OnRunPressed;
    public UnityEvent OnCrouchPressed;
    public UnityEvent OnInteractPressed;
    public UnityEvent OnCameraOpened;

    [Header("Camera Input Events")]
    public UnityEvent OnPhotoTaken;

    private void Awake()
    {
        if (inputReader != null)
        {
            inputReader.Initialize();

            // Subscribe to player events
            inputReader.JumpEvent.AddListener(() => OnJumpPressed?.Invoke());
            inputReader.RunEvent.AddListener(() => OnRunPressed?.Invoke());
            inputReader.CrouchEvent.AddListener(() => OnCrouchPressed?.Invoke());
            inputReader.InteractEvent.AddListener(() => OnInteractPressed?.Invoke());
        }
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.EnablePlayerInput();
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.DisablePlayerInput();
        }
    }

    private void LateUpdate()
    {
        if (inputReader != null)
            inputReader.ClearOneFrameInputFlags();
    }

    private void OnDestroy()
    {
        if (inputReader != null)
            inputReader.ResetValues();
    }
}