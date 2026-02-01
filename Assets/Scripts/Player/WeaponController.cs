using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private TrailRenderer bulletTrail;

    [Header("Weapon Settings")]
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletSpreadAngle = 0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 20f;

    public event Action OnShoot;
    public event Action OnShootProcessed;

    public ParticleSystem muzzleFlash;
    public Light flash;

    [SerializeField] private float minFlashDuration = 0.03f;
    [SerializeField] private float maxFlashDuration = 0.07f;

    public int CurrentAmmo { get; private set; } = 10;
    public GameObject Owner { get; set; }


    private bool triggerHeld;
    private float nextFireTime;

    private void Awake()
    {
        Owner = gameObject;

        if (flash != null)
        {
            flash.enabled = false;
        }
    }

    private void Update()
    {
        if (triggerHeld)
            TryShoot();
    }

    public void SetTriggerHeld(bool held)
    {
        triggerHeld = held;
    }

    private void TryShoot()
    {
        if (CurrentAmmo <= 0)
            return;

        if (Time.time < nextFireTime)
            return;

        HandleShoot();
        nextFireTime = Time.time + fireRate;
    }

    private void HandleShoot()
    {
        Vector3 origin = shootPoint.position;

        if (flash != null)
        {
            StartCoroutine(FlashLightRoutine());
        }

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 direction = GetDirectionWithinSpread(shootPoint.forward);
            Vector3 endPoint = origin + direction * range;

            bool hitSomething = Physics.Raycast(
                origin,
                direction,
                out RaycastHit hit,
                range
            );

            if (hitSomething)
            {
                endPoint = hit.point;

                if (IsHitValid(hit))
                    HandleHit(hit);
            }

            SpawnTracer(origin, endPoint);
        }

        CameraShake.Instance.ShakeCamera(3.5f, .35f);
        if (muzzleFlash != null ) muzzleFlash.Play();

        // TODO: subtract ammo here

        OnShoot?.Invoke();
        OnShootProcessed?.Invoke();
    }

    private Vector3 GetDirectionWithinSpread(Vector3 forward)
    {
        if (bulletSpreadAngle <= 0f)
            return forward;

        float spreadRatio = bulletSpreadAngle / 180f;
        return Vector3.Slerp(forward, UnityEngine.Random.insideUnitSphere, spreadRatio).normalized;
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            return false;

        return true;
    }

    private void HandleHit(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Damageable damageable))
            damageable.InflictDamage(damage, Owner);

        // impact VFX / SFX here
        
    }

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        TrailRenderer trail = Instantiate(bulletTrail, start, Quaternion.identity);
        StartCoroutine(AnimateTrail(trail, start, end));
    }

    private IEnumerator AnimateTrail(TrailRenderer trail, Vector3 start, Vector3 end)
    {
        float travelTime = 0.15f; // visual bullet speed
        float elapsed = 0f;

        while (elapsed < travelTime)
        {
            trail.transform.position = Vector3.Lerp(start, end, elapsed / travelTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = end;
        Destroy(trail.gameObject, trail.time);
    }

    private IEnumerator FlashLightRoutine()
    {
        flash.enabled = true;

        float duration = UnityEngine.Random.Range(minFlashDuration, maxFlashDuration);
        yield return new WaitForSeconds(duration);

        flash.enabled = false;
    }
}
