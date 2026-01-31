using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Awake()
    {
        if (inputReader != null)
        {
            inputReader.Initialize();
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