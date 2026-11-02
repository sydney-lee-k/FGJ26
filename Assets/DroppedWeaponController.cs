using System;
using Unity.VisualScripting;
using UnityEngine;

// Might be good to make this as an IPoolable and then dynamically change models. Easier to test once we actually have models tho so for now instantiated.
public class DroppedWeaponController : MonoBehaviour
{
    public WeaponController weapon;
    #if UNITY_EDITOR
        [ReadOnly] [SerializeField] private int currentAmmo;
    #endif
    public PlayerWeaponsManager playerWeaponsManager;
    [SerializeField] private float pickupRange = 2f;
    private Transform player;
    private float checkDelay;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerWeaponsManager = player.GetComponent<PlayerWeaponsManager>();
    }

    private void Start()
    {
        Physics.SyncTransforms();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        currentAmmo = weapon.currentAmmo;
        #endif
        
        if (checkDelay <= 0)
        {
            if (Vector3Utils.IsWithinDistance(player.position, transform.position, pickupRange))
            {
                if (weapon.currentAmmo > 0)
                {
                    WeaponController activeWeapon = playerWeaponsManager.GetActiveWeapon();
                    if (weapon.weaponId == activeWeapon.weaponId && activeWeapon.currentAmmo != activeWeapon.MaxAmmo)
                    {
                        //Formula can be changed here.
                        int missingAmmo = activeWeapon.MaxAmmo - activeWeapon.currentAmmo;
                        int ammoToRemove = Mathf.Min(weapon.currentAmmo, missingAmmo);
                        weapon.currentAmmo -= ammoToRemove;
                        activeWeapon.currentAmmo += ammoToRemove;
                    }
                }
                else
                {
                    //Remove pickup and shit.
                }
            }

            checkDelay = 0.5f;
        }
        else
        {
            checkDelay -= Time.deltaTime;
        }
    }
    
    public void Pickup()
    {
        PlayerWeaponsManager playerWeaponsManager = player.GetComponent<PlayerWeaponsManager>();
        bool success = playerWeaponsManager.AddWeapon(weapon);
        //Debug.Log("tried to add weapon: " + success);
        if (!success) return;
        Destroy(gameObject);
    }
}

