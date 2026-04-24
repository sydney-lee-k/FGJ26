using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Tooltip("Multiplier to apply to the received damage")]
    public float DamageMultiplier = 1f;

    public Health Health { get; private set; }

    void Awake()
    {
        Health = GetComponent<Health>();
        if (!Health)
        {
            Health = GetComponentInParent<Health>();
        }
    }

    public void TakeDamage(float damage, GameObject damageSource)
    {
        if (Health)
        {
            Health.TakeDamage(damage, damageSource);
        }
    }
}