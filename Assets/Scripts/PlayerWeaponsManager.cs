using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponsManager : MonoBehaviour, IWeaponUser
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Weapons")]
    public List<WeaponController> StartingWeapons = new();
    [SerializeField] private WeaponController[] WeaponSlots = new WeaponController[2];

    [Header("References")]
    [SerializeField] private Transform weaponMuzzle;
    [SerializeField] private Actor actor;

    public Transform WeaponParent;

    public Actor Owner => actor;
    public Transform AimOrigin => weaponMuzzle;
    public Vector3 AimDirection => transform.forward;
    private int activeWeaponIndex;

    private WeaponController ActiveWeapon =>
        (WeaponSlots != null && WeaponSlots.Length > 0) ? WeaponSlots[activeWeaponIndex] : null;

    private void Awake()
    {
        // Initialize weapons
        foreach (var weapon in StartingWeapons)
        {
            AddWeapon(weapon);
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
        if (WeaponSlots.Length < 2)
            return;

        if (ActiveWeapon != null)
            ActiveWeapon.SetFireHeld(false);

        int startIndex = activeWeaponIndex;

        for (int i = 1; i <= WeaponSlots.Length; i++)
        {
            int nextIndex = (startIndex + i) % WeaponSlots.Length;

            if (WeaponSlots[nextIndex] != null)
            {
                SetActiveWeapon(nextIndex);
                return;
            }
        }
    }

    private void SetActiveWeapon(int index)
    {
        if (index < 0 || index >= WeaponSlots.Length)
            return;

        if (ActiveWeapon != null)
        {
            ActiveWeapon.SetFireHeld(false);
            ActiveWeapon.gameObject.SetActive(false);
        }

        activeWeaponIndex = index;

        WeaponSlots[activeWeaponIndex].gameObject.SetActive(true);
    }

    public bool AddWeapon(WeaponController prefab, int slotIndex = -1, bool autoEquip = true)
    {
        if (prefab == null)
            return false;

        // Decide slot
        if (slotIndex < 0 || slotIndex >= WeaponSlots.Length)
        {
            slotIndex = GetFirstEmptySlot();

            if (slotIndex == -1)
                slotIndex = activeWeaponIndex;
        }

        // Remove existing weapon in slot
        RemoveWeapon(slotIndex);

        // INSTANTIATE HERE (your requirement)
        WeaponController instance = Instantiate(prefab, WeaponParent);
        instance.SetUser(this);
        instance.gameObject.SetActive(false);

        WeaponSlots[slotIndex] = instance;

        if (autoEquip)
            SetActiveWeapon(slotIndex);

        return true;
    }

    public void RemoveWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= WeaponSlots.Length)
            return;

        WeaponController weapon = WeaponSlots[slotIndex];

        if (weapon == null)
            return;

        if (slotIndex == activeWeaponIndex)
            weapon.SetFireHeld(false);

        WeaponSlots[slotIndex] = null;

        Destroy(weapon.gameObject);
    }

    public WeaponController GetActiveWeapon()
    {
        return ActiveWeapon;
    }

    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < WeaponSlots.Length; i++)
        {
            if (WeaponSlots[i] == null)
                return i;
        }

        return -1;
    }
}