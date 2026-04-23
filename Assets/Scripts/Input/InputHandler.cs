using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Awake()
    {
        inputReader.Initialize();
        inputReader.EnablePlayerInput();
    }

    private void OnDisable()
    {
        inputReader.DisablePlayerInput();
    }
}