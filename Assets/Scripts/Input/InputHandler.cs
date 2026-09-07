using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Start()
    {
        inputReader = InputReader.instance;
        inputReader.EnablePlayerInput();
    }

    private void OnDisable()
    {
        inputReader.DisablePlayerInput();
    }
}