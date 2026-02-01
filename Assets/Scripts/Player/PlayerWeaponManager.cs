using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private WeaponController weapon;

    private void Update()
    {
        weapon.SetTriggerHeld(inputReader.ShootHeld);
    }
}
