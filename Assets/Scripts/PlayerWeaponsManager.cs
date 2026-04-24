using UnityEngine;

public class PlayerWeaponsManager : MonoBehaviour, IWeaponUser
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Weapons")]
    [SerializeField] private WeaponController[] weapons = new WeaponController[2];

    [Header("References")]
    [SerializeField] private Transform weaponMuzzle;
    [SerializeField] private Actor actor;

    public Actor Owner => actor;
    public Transform AimOrigin => weaponMuzzle;
    public Vector3 AimDirection => transform.forward;

    private int activeWeaponIndex;
    private WeaponController ActiveWeapon => weapons[activeWeaponIndex];

    private void Awake()
    {
        // Initialize weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null) continue;

            weapons[i].SetUser(this);
            weapons[i].gameObject.SetActive(i == activeWeaponIndex);
        }
    }

    private void OnEnable()
    {
        inputReader.AttackInputDown += HandleAttack;
        inputReader.SwitchPressed += SwitchWeapon;
    }

    private void OnDisable()
    {
        inputReader.AttackInputDown -= HandleAttack;
        inputReader.SwitchPressed -= SwitchWeapon;
    }

    private void HandleAttack(bool held)
    {
        if (ActiveWeapon != null)
            ActiveWeapon.SetFireHeld(held);
    }

    private void SwitchWeapon()
    {
        if (weapons.Length < 2)
            return;

        // Stop firing current weapon immediately
        if (ActiveWeapon != null)
            ActiveWeapon.SetFireHeld(false);

        // Disable current weapon
        weapons[activeWeaponIndex].gameObject.SetActive(false);

        // Switch index
        activeWeaponIndex = (activeWeaponIndex + 1) % weapons.Length;

        // Enable new weapon
        weapons[activeWeaponIndex].gameObject.SetActive(true);
    }
}