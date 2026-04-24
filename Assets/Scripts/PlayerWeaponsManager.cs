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

    private WeaponController ActiveWeapon =>
        (weapons != null && weapons.Length > 0) ? weapons[activeWeaponIndex] : null;

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

    public void AddWeapon(WeaponController newWeapon, int slotIndex, bool autoEquip = true)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
        {
            Debug.LogWarning("Invalid weapon slot index");
            return;
        }

        // Remove existing weapon in slot
        RemoveWeapon(slotIndex);

        weapons[slotIndex] = newWeapon;

        if (newWeapon == null) return;

        newWeapon.SetUser(this);
        newWeapon.transform.SetParent(transform);

        newWeapon.gameObject.SetActive(false);

        if (autoEquip)
        {
            EquipWeapon(slotIndex);
        }
    }

    public void RemoveWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
            return;

        WeaponController weapon = weapons[slotIndex];

        if (weapon == null) return;

        // If removing active weapon, stop firing
        if (slotIndex == activeWeaponIndex)
        {
            weapon.SetFireHeld(false);
        }

        weapon.gameObject.SetActive(false);

        // Optional: destroy or detach
        Destroy(weapon.gameObject);

        weapons[slotIndex] = null;
    }

    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
            return;

        if (weapons[slotIndex] == null)
            return;

        if (ActiveWeapon != null)
        {
            ActiveWeapon.SetFireHeld(false);
            ActiveWeapon.gameObject.SetActive(false);
        }

        activeWeaponIndex = slotIndex;

        weapons[activeWeaponIndex].gameObject.SetActive(true);
    }
}