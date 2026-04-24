using UnityEngine;
using UnityEngine.Events;

public enum FireMode
{
    Manual,
    Automatic
}

public enum WeaponCycleType
{
    Instant,
    Pump
}

public class WeaponController : MonoBehaviour
{
    [Header("Firing")]
    [SerializeField] private FireMode fireMode = FireMode.Manual;
    [SerializeField] private WeaponCycleType cycleType = WeaponCycleType.Instant;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo = 30;

    [Header("Casings")]
    [SerializeField] private bool useCasings;
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private Transform casingEjectPoint;

    [Header("Stats")]
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private float range = 50f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float spreadAngle = 0f;

    [Header("Hit Settings")]
    [SerializeField] private LayerMask hitMask;

    public UnityAction OnShoot;


    private bool waitingForPumpEject;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    private IWeaponUser user;

    private bool fireHeld;
    private bool firePressed;

    private float lastFireTime;

    public bool HasAmmo => currentAmmo > 0;

    private void Awake()
    {
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
    }

    private void Update()
    {
        HandleShootInputs();
    }

    private void ShootShell()
    {
        if (casingPrefab == null || casingEjectPoint == null)
            return;

        GameObject casing = Instantiate(casingPrefab, casingEjectPoint.position, casingEjectPoint.rotation);
        
        if (casing.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(casingEjectPoint.right * Random.Range(1.5f, 3f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }
    }

    private void HandleShellEjection()
    {
        switch (cycleType)
        {
            case WeaponCycleType.Instant:
                ShootShell();
                break;

            case WeaponCycleType.Pump:
                waitingForPumpEject = true;
                break;
        }
    }

    public void PumpAction()
    {
        if (cycleType != WeaponCycleType.Pump)
            return;

        if (waitingForPumpEject)
        {
            ShootShell();
            waitingForPumpEject = false;
        }
    }

    public void SetUser(IWeaponUser weaponUser)
    {
        user = weaponUser;
    }

    public void SetFireHeld(bool held)
    {
        fireHeld = held;

        if (held)
            firePressed = true;
    }

    public void HandleShootInputs()
    {
        switch (fireMode)
        {
            case FireMode.Manual:
                if (firePressed)
                    TryFire();
                break;

            case FireMode.Automatic:
                if (fireHeld)
                    TryFire();
                break;

            default:
                break;
        }

        firePressed = false;
    }

    private bool TryFire()
    {
        if (!HasAmmo)
            return false;

        if (Time.time < lastFireTime + 1f / fireRate)
            return false;

        lastFireTime = Time.time;
        currentAmmo--;

        Fire();

        return true;
    }

    private void Fire()
    {
        Vector3 origin = user.AimOrigin.position;
        Vector3 baseDirection = user.AimDirection;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetDirectionWithinSpread(baseDirection, spreadAngle);
            if (Physics.Raycast(origin, shotDirection, out RaycastHit hit, range, hitMask))
            {
                // Validate hit first
                if (!IsHitValid(hit))
                    continue;

                OnHit(hit);
            }
        }

        // muzzle flash

        if (useCasings)
            HandleShellEjection();

        // shoot sfx

        // weapon animation (if any)

        OnShoot?.Invoke();
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
        if (hit.collider.TryGetComponent<Damageable>(out var damageable))
        {
            damageable.TakeDamage(damage, gameObject);
        }

        // impact vfx
        // impact sfx
    }

    private Vector3 GetDirectionWithinSpread(Vector3 direction, float angle)
    {
        if (angle <= 0f)
            return direction;

        Vector2 randomPoint = Random.insideUnitCircle * angle;

        Quaternion spreadRotation = Quaternion.Euler(
            randomPoint.y,
            randomPoint.x,
            0f
        );

        return spreadRotation * direction;
    }
}