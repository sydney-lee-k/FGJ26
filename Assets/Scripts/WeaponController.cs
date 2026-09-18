using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

//Manual overrides ShotDelay if you spam M1.

public enum FireMode
{
    Manual,
    Automatic,
    Melee
}


public class WeaponController : MonoBehaviour
{
    [Header("Firing")]
    [SerializeField] private FireMode fireMode = FireMode.Manual;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo = 30;

    [Header("Casings")]
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private Transform casingEjectPoint;
    
    [Header("Casings")]
    [SerializeField] private GameObject tracer;

    [Header("Stats")]
    [SerializeField] private float shotDelay = 0.05f;
    private float remainingShotDelay;
    [SerializeField] private float ejectDelay = 0f;
    private float remainingEjectDelay;
    private bool preparingToEject;
    [SerializeField] private float range = 50f;
    [SerializeField] private float thickness = 0f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float spreadAngle = 0f;
    [SerializeField] private float noiseRange = 0;

    [Header("Hit Settings")]
    [SerializeField] private LayerMask hitMask;
    
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    private IWeaponUser user;
    [SerializeField] private bool firePressed;

    private void Awake()
    {
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
    }

    public void SetUser(IWeaponUser weaponUser)
    {
        user = weaponUser;
    }

    public void SetFiring(bool held)
    {
        if (!firePressed && fireMode == FireMode.Manual) //No shotDelay on automatic weapons. Shoot as fast as you spam.
        {
            remainingShotDelay = 0;
        }
        
        firePressed = held;
    }
    
    private void Update()
    {
        if (remainingShotDelay > 0)
        {
            remainingShotDelay -= Time.deltaTime;
        } else if (firePressed)
        {
            if (!TryFire())
            {
                //Play empty gun sound.
            }
        }
        
        if (remainingEjectDelay > 0)
        {
            remainingEjectDelay -= Time.deltaTime;
        }
        else if(preparingToEject)
        {
            preparingToEject = false;
            ShootShell();
        }
    }
    
    private bool TryFire()
    {
        if (currentAmmo > 0) currentAmmo--;
        else return false;

        Fire();
        return true;
    }

    private void Fire()
    {
        Vector3 origin = user.AimOrigin.position;
        Vector3 baseDirection = user.AimDirection;
        if(noiseRange > 0) NoiseController.Instance.CreateNoise(origin, noiseRange);
        remainingShotDelay = shotDelay;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetDirectionWithinSpread(baseDirection, spreadAngle);
            Vector3 tracerHitPosition = origin + shotDirection * range;

            if (thickness <= 0)
            {
                if (Physics.Raycast(origin, shotDirection, out RaycastHit hit, range, hitMask))
                {
                    tracerHitPosition = hit.point;

                    if (IsHitValid(hit))
                        OnHit(hit);
                }
            }
            else
            {
                //Melee / Thick shots
                if (Physics.SphereCast(origin,  thickness, shotDirection, out RaycastHit hit, range, hitMask))
                {
                    tracerHitPosition = hit.point;

                    if (IsHitValid(hit))
                        OnHit(hit);
                }
            }

            if (tracer)
            {
                Tracer trace = PoolManager.Instance.Spawn(tracer, user.AimOrigin.position + user.AimDirection.normalized*thickness, quaternion.identity).GetComponent<Tracer>();
                trace.transform.localEulerAngles = user.AimDirection;
                trace.Initialize(tracerHitPosition);
            }
        }
        
        if (casingPrefab)
        {
            preparingToEject = true;
            remainingEjectDelay = ejectDelay;
        }
        
        // shoot sfx
        // weapon animation (if any)
    }
    
    private void ShootShell()
    {
        if (casingEjectPoint == null)
        {
            Debug.Log("Missing casing eject point");
            return;
        }

        GameObject casing = PoolManager.Instance.Spawn(casingPrefab, casingEjectPoint.position, casingEjectPoint.rotation);
        
        if (casing.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(casingEjectPoint.right * Random.Range(1.5f, 3f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }
    }
    

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger)
            return false;

        if (hit.collider.transform.IsChildOf(user.Owner.transform))
            return false;

        return true;
    }

    private void OnHit(RaycastHit hit)
    {
        if(user.Owner.affiliation == Actor.Affiliation.Player) Debug.Log("Hit");
        if (hit.collider.TryGetComponent<Damageable>(out var damageable))
        {
            damageable.TakeDamage(damage, gameObject);
        }

        // impact vfx
        // impact sfx
    }

    private Vector3 GetDirectionWithinSpread(Vector3 direction, float angle)
    {
        if (angle <= 0f) return direction;
        Vector2 randomPoint = Random.insideUnitCircle * angle;
        Quaternion spreadRotation = Quaternion.Euler(randomPoint.y, randomPoint.x, 0f);
        return spreadRotation * direction;
    }

    private void OnDrawGizmos()
    {
        if (firePressed && fireMode == FireMode.Melee)
        {
            Gizmos.color = Color.yellow;
            Vector3 origin = user.AimOrigin.position;
            Gizmos.DrawWireSphere(origin+user.AimDirection.normalized*thickness, thickness);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin+user.AimDirection.normalized*range, thickness);
        }
    }
}