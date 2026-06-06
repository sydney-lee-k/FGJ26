using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponController weaponPrefab;

    [Header("Settings")]
    [SerializeField] private int forcedSlot = -1;
    [SerializeField] private bool autoEquip = true;

    public void OnPicked(GameObject byPlayer)
    {
        if (byPlayer == null)
            return;

        if (byPlayer.TryGetComponent<PlayerWeaponsManager>(out var weapons))
        {
            bool success = weapons.AddWeapon(weaponPrefab, forcedSlot, autoEquip);

            if (!success)
                return;

            //Destroy(gameObject);
        }
    }
}