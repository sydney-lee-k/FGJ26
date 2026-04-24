using UnityEngine;

public class PlayerWeaponsManager : MonoBehaviour, IWeaponUser
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private WeaponController weapon;
    [SerializeField] private Transform weaponMuzzle;
    [SerializeField] private Actor actor;

    public Actor Owner => actor;
    public Transform AimOrigin => weaponMuzzle;
    public Vector3 AimDirection => transform.forward;

    private void Awake()
    {
        weapon.SetUser(this);
    }

    private void OnEnable()
    {
        inputReader.AttackInputChanged += weapon.SetFireHeld;
    }

    private void OnDisable()
    {
        inputReader.AttackInputChanged -= weapon.SetFireHeld;
    }
}