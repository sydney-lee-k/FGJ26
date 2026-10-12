using System;
using UnityEngine;

// Might be good to make this as an IPoolable and then dynamically change models. Easier to test once we actually have models tho so for now instantiated.
public class DroppedWeaponController : MonoBehaviour
{
    public WeaponController weapon;
    public int remainingAmmo;
    public PlayerWeaponsManager playerWeaponsManager;
    [SerializeField] private float pickupRange = 2f; 
    private Transform player;
    private float checkDelay;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerWeaponsManager = player.GetComponent<PlayerWeaponsManager>();
    }

    private void Update()
    {
        if (checkDelay <= 0)
        {
            if (Vector3Utils.IsWithinDistance(player.position, transform.position, pickupRange))
            {
                //Add weapon pickup here? Or use old interactive systme. We shall see.
            
                if (weapon.currentAmmo > 0)
                {
                    WeaponController activeWeapon = playerWeaponsManager.GetActiveWeapon();
                    if (weapon.ammoType == activeWeapon.ammoType && activeWeapon.currentAmmo != activeWeapon.MaxAmmo)
                    {
                        //Formula can be changed here.
                        int missingAmmo = activeWeapon.MaxAmmo - activeWeapon.currentAmmo;
                        int ammoToRemove = Mathf.Min(remainingAmmo, missingAmmo);
                        remainingAmmo -= ammoToRemove;
                        activeWeapon.currentAmmo += ammoToRemove;
                    }
                }
            }

            checkDelay = 0.5f;
        }
        else
        {
            checkDelay -= Time.deltaTime;
        }
    }
}
