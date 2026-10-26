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
    
    public Actor Owner => actor;
    public Transform AimOrigin => actor.AimPoint;
    public Vector3 AimDirection => actor ? target.transform.position - AimOrigin.position : transform.forward;
    private void Awake()
    {
        SwapWeapon(primaryWeapon);
        if(!spawnParent) spawnParent = transform;
    }
    
    private void Update()
    {
        if (target) currentWeapon.SetFiring(true);
        else currentWeapon.SetFiring(false);
        
    }
    
    public void SwapWeapon(WeaponController prefab)
    {
        if(currentWeapon) Destroy(currentWeapon);
        currentWeapon = Instantiate(prefab, spawnParent);
        currentWeapon.SetUser(this);
    }

    public void DropWeapon()
    {
        GameObject droppedWeapon = Instantiate(currentWeapon.transform.GetChild(0).gameObject, currentWeapon.transform.position, currentWeapon.transform.rotation, null);
        DroppedWeaponController droppedWeaponController = droppedWeapon.GetComponent<DroppedWeaponController>();
        droppedWeaponController.enabled = true;
        droppedWeaponController.weapon = primaryWeapon;
        droppedWeaponController.remainingAmmo = currentWeapon.currentAmmo;
        droppedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        droppedWeapon.GetComponent<Interactable>().SetInteractable(true);
        droppedWeapon.GetComponentInChildren<MeshCollider>().isTrigger = false;

    }
}
