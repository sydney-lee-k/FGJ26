using UnityEngine;

public class PlayerWeapons : MonoBehaviour, IWeaponUser
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private WeaponController weapon;
    [SerializeField] private Transform weaponMuzzle;

    private bool isFiring;

    public Transform AimOrigin => weaponMuzzle;

    public Vector3 AimDirection => transform.forward;

    private void OnEnable()
    {
        inputReader.AttackHeld += HandleAttack;
    }

    private void OnDisable()
    {
        inputReader.AttackHeld -= HandleAttack;
    }

    private void Update()
    {
        if (isFiring)
        {
            weapon.TryFire(this);
        }
    }

    private void HandleAttack(bool held)
    {
        isFiring = held;
    }
}