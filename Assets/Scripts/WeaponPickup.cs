using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponController weaponPrefab;

    public void OnPicked(GameObject byPlayer)
    {
        if (byPlayer == null)
            return;

        if (byPlayer.TryGetComponent<PlayerWeaponsManager>(out var weapons))
        {
            bool success = weapons.AddWeapon(weaponPrefab);

            if (!success)
                return;

            //Destroy(gameObject);
        }
    }
}