using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private float range = 50f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField] private float spreadAngle = 0f;

    [Header("Hit Settings")]
    [SerializeField] private LayerMask hitMask;

    private float lastFireTime;

    public void TryFire(IWeaponUser user)
    {
        if (Time.time < lastFireTime + 1f / fireRate)
            return;

        Fire(user);
        lastFireTime = Time.time;
    }

    private void Fire(IWeaponUser user)
    {
        Vector3 origin = user.AimOrigin.position;
        Vector3 baseDirection = user.AimDirection;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetDirectionWithinSpread(baseDirection, spreadAngle);
            if (Physics.Raycast(origin, shotDirection, out RaycastHit hit, range, hitMask))
            {
                // Validate hit first
                if (!IsHitValid(hit, user))
                {
                    Debug.DrawLine(origin, hit.point, Color.gray, 0.1f);
                    continue;
                }

                OnHit(hit);

                // Debug
                Debug.DrawLine(origin, hit.point, Color.red, 0.2f);
            }
            else
            {
                Debug.DrawRay(origin, shotDirection * range, Color.yellow, 0.2f);
            }
        }       
    }

    private bool IsHitValid(RaycastHit hit, IWeaponUser user)
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

        return Quaternion.Slerp(
            Quaternion.LookRotation(direction),
            Random.rotation,
            angle / 180f
        ) * Vector3.forward;
    }
}
