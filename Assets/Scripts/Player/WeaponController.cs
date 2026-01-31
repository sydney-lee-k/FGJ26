using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public InputReader inputReader;

    public Transform shootPoint;
    public int bulletsPerShot;


    public TrailRenderer bulletTrail;

    public float fireRate = 0.2f;
    public float bulletSpreadAngle = 0f;

    public float damage = 10f;
    public float range = 20f;

    public UnityAction OnShoot;
    public event Action OnShootProcessed;

    private List<Collider> m_ignoredColliders;

    public int m_currentAmmo = 10;

    public GameObject Owner { get; set; }

    private float m_nextTimeToFire = Mathf.NegativeInfinity;


    private void Start()
    {
        Owner = gameObject;
    }

    private void Update()
    {
        if (inputReader.ShootHeld)
        {
            TryShoot();
        }
    }

    private bool TryShoot()
    {
        if (m_currentAmmo >= 1 && Time.time >= m_nextTimeToFire)
        {
            HandleShoot();
            // minus ammo

            return true;
        }

        return false;
    }

    private void HandleShoot()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 t_direction = GetDirectionWithinSpread(shootPoint);

            Debug.DrawRay(shootPoint.position, t_direction * range, Color.blue, 1.0f);
            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, range))
            {
                TrailRenderer trail = Instantiate(bulletTrail, shootPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));

                if (IsHitValid(hit))
                {
                    OnHit(hit.point, hit.normal, hit.collider);
                }
            }
        }

        m_nextTimeToFire = Time.time + fireRate;

        OnShoot?.Invoke();
        OnShootProcessed?.Invoke();
    }

    public Vector3 GetDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = bulletSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);

        return spreadWorldDirection;
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
        {
            return false;
        }

        if (m_ignoredColliders != null && m_ignoredColliders.Contains(hit.collider))
        {
            return false;
        }

        return true;
    }

    private void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        Damageable damageable = collider.GetComponent<Damageable>();
        if (damageable)
        {
            damageable.InflictDamage(damage, gameObject);
        }

        //impact vfx and sfx
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        //Instantiate(particleSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }
}