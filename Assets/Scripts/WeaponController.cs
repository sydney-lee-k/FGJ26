using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float fireRate = 5f;

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
        Debug.DrawRay(
            user.AimOrigin.position,
            user.AimDirection * 5f,
            Color.red,
            0.5f
        );
    }
}
