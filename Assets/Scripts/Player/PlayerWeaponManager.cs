using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private WeaponController weapon1;
    [SerializeField] private WeaponController weapon2;

    private void Update()
    {
        if (inputReader.QPressedThisFrame)
        {
            weapon1.gameObject.SetActive(!weapon1.gameObject.activeSelf);
            weapon2.gameObject.SetActive(!weapon1.gameObject.activeSelf);
        }

        weapon1.SetTriggerHeld(inputReader.ShootHeld);
        weapon2.SetTriggerHeld(inputReader.ShootHeld);
    }
}