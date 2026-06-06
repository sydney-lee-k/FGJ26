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

    public Transform WeaponParent;


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

        if (ActiveWeapon != null)
            ActiveWeapon.SetFireHeld(false);

        weapons[activeWeaponIndex].gameObject.SetActive(false);

        int nextIndex = (activeWeaponIndex + 1) % weapons.Length;

        // skip empty slots safely
        for (int i = 0; i < weapons.Length; i++)
        {
            int checkIndex = (activeWeaponIndex + 1 + i) % weapons.Length;

            if (weapons[checkIndex] != null)
            {
                nextIndex = checkIndex;
                break;
            }
        }

        activeWeaponIndex = nextIndex;
        weapons[activeWeaponIndex].gameObject.SetActive(true);
    }

    public WeaponController GetActiveWeapon()
    {
        return ActiveWeapon;
    }

    public void ForceSwitchIfEmpty()
    {
        if (ActiveWeapon == null)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    EquipWeapon(i);
                    return;
                }
            }
        }
    }

    public bool AddWeapon(WeaponController newWeapon, int slotIndex = -1, bool autoEquip = true)
    {
        if (newWeapon == null)
            return false;

        // Decide slot
        if (slotIndex < 0 || slotIndex >= weapons.Length)
        {
            slotIndex = GetFirstEmptySlot();

            if (slotIndex == -1)
                slotIndex = activeWeaponIndex;
        }

        // Remove existing weapon in slot
        RemoveWeapon(slotIndex);

        // Assign new weapon
        weapons[slotIndex] = newWeapon;

        newWeapon.SetUser(this);
        newWeapon.transform.SetParent(transform);
        newWeapon.gameObject.SetActive(false);

        if (autoEquip)
        {
            EquipWeapon(slotIndex);
        }

        return true;
    }

    public void RemoveWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
            return;

        WeaponController weapon = weapons[slotIndex];

        if (weapon == null) return;

        if (slotIndex == activeWeaponIndex)
        {
            weapon.SetFireHeld(false);
        }

        weapon.gameObject.SetActive(false);

        Destroy(weapon.gameObject);

        weapons[slotIndex] = null;
    }

    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weapons.Length)
            return;

        if (weapons[slotIndex] == null)
            return;

        ActiveWeapon.SetFireHeld(false);
        ActiveWeapon.gameObject.SetActive(false);

        activeWeaponIndex = slotIndex;

        weapons[activeWeaponIndex].gameObject.SetActive(true);
    }

    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                return i;
        }

        return -1;
    }
}