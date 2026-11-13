using System;
using Unity.Mathematics;
using UnityEngine;
public class EnemyWeaponController : MonoBehaviour, IWeaponUser
{
    [Header("Weapon")]
    public WeaponController primaryWeapon;
    private WeaponController currentWeapon;
    
    [Header("Variables")]
    [SerializeField] private Actor actor;
    [NonSerialized] public Actor target; //When target is set will start to fire.
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float reloadTime = 1;
    [SerializeField] [ReadOnly] private float timeSpentReloading;
    [SerializeField] private EnemyController.SpeedMultiplier reloadSpeedMultiplier;
    private EnemyController enemyController;
    
    public Actor Owner => actor;
    public Transform AimOrigin => actor.AimPoint;
    public Vector3 AimDirection => actor ? target.transform.position - AimOrigin.position : transform.forward;
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        if(!spawnParent) spawnParent = transform;
        SwapWeapon(primaryWeapon);
    }
    
    private void Update()
    {
        if (currentWeapon.currentAmmo > 0)
        {
            if (target) currentWeapon.SetFiring(true);
            else currentWeapon.SetFiring(false);
        }
        else if(timeSpentReloading < reloadTime)
        {
            if (timeSpentReloading == 0)
            {
                currentWeapon.SetFiring(false);
                enemyController.AddGlobalSpeedMultiplier(reloadSpeedMultiplier);
            }
            timeSpentReloading += Time.deltaTime;
        }
        else
        {
            if(timeSpentReloading == 0) enemyController.RemoveGlobalSpeedMultiplier(reloadSpeedMultiplier.id);
            currentWeapon.currentAmmo = currentWeapon.MaxAmmo;
            timeSpentReloading = 0;
        }
    }
    
    public void SwapWeapon(WeaponController prefab)
    {
        if(currentWeapon) Destroy(currentWeapon);
        currentWeapon = Instantiate(prefab, spawnParent);
        currentWeapon.SetUser(this);
    }

    public void DropWeapon()
    {
        if (!currentWeapon.weaponVisualPrefab) return;
        GameObject droppedWeapon = Instantiate(currentWeapon.weaponVisualPrefab, currentWeapon.transform.position, currentWeapon.transform.rotation, null);
        DroppedWeaponController droppedWeaponController = droppedWeapon.GetComponent<DroppedWeaponController>();
        droppedWeaponController.currentAmmo = currentWeapon.currentAmmo;
        droppedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        droppedWeapon.GetComponent<Interactable>().SetInteractable(true);
        droppedWeapon.GetComponentInChildren<MeshCollider>().isTrigger = false;
        droppedWeaponController.enabled = true;

    }
}
