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
            WeaponController instance = Instantiate(weaponPrefab);

            if (!weapons.AddWeapon(instance))
            {
                //Destroy(instance.gameObject);
                return;
            }

            //Destroy(gameObject);
        }
    }
}