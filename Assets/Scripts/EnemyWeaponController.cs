using System;
using Unity.Mathematics;
using UnityEngine;
public class EnemyWeaponController : MonoBehaviour, IWeaponUser
{
    [Header("Weapon")]
    public WeaponController startingWeapon;
    [SerializeField] private GameObject droppedWeaponPrefab;
    private WeaponController currentWeapon;
    
    [Header("Variables")]
    [SerializeField] private Actor actor;
    [NonSerialized] public Actor target; //When target is set will start to fire.
    
    public Actor Owner => actor;
    public Transform AimOrigin => actor.AimPoint;
    public Vector3 AimDirection => actor ? target.transform.position - AimOrigin.position : transform.forward;
    private void Awake()
    {
        SwapWeapon(startingWeapon);
    }
    
    private void Update()
    {
        if (target) currentWeapon.SetFiring(true);
        else currentWeapon.SetFiring(false);
        
    }
    
    public void SwapWeapon(WeaponController prefab)
    {
        if(currentWeapon) Destroy(currentWeapon);
        currentWeapon = Instantiate(prefab, transform);
        currentWeapon.SetUser(this);
    }

    public void DropWeapon()
    {
        if (!droppedWeaponPrefab) return;
        DroppedWeaponController droppedWeapon = Instantiate(droppedWeaponPrefab, transform.position, quaternion.identity).GetComponent<DroppedWeaponController>();
        droppedWeapon.weapon = startingWeapon;
        droppedWeapon.remainingAmmo = currentWeapon.currentAmmo;

    }
}
