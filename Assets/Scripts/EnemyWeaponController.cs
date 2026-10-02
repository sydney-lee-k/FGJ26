using System;
using UnityEngine;
public class EnemyWeaponController : MonoBehaviour, IWeaponUser
{
    [Header("Weapon")]
    public WeaponController startingWeapon;
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
        if (target)
        {
            currentWeapon.SetFiring(true);
        }
        else
        {
            currentWeapon.SetFiring(false);
        }
    }
    
    public void SwapWeapon(WeaponController prefab)
    {
        if(currentWeapon) Destroy(currentWeapon);
        currentWeapon = Instantiate(prefab, transform);
        currentWeapon.SetUser(this);
    }
}
