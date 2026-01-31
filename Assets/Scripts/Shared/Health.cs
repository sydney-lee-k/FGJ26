using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10f;

    [Tooltip("Health ratio at which the critical health state starts")]
    [SerializeField] private float criticalHealthRatio = 0.3f;

    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction<float> OnHealed;
    public UnityAction OnDie;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    public bool Invincible { get; set; }
    public bool IsDead => isDead;

    private bool isDead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public bool CanPickup() => CurrentHealth < maxHealth;

    public float GetRatio()
    {
        return maxHealth > 0f ? CurrentHealth / maxHealth : 0f;
    }

    public bool IsCritical()
    {
        return GetRatio() <= criticalHealthRatio;
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        float before = CurrentHealth;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);

        float healed = CurrentHealth - before;
        if (healed > 0f)
        {
            OnHealed?.Invoke(healed);
        }
    }

    public void TakeDamage(float amount, GameObject source)
    {
        if (isDead || Invincible || amount <= 0f)
            return;

        float before = CurrentHealth;
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);

        float damageTaken = before - CurrentHealth;
        if (damageTaken > 0f)
        {
            OnDamaged?.Invoke(damageTaken, source);
        }

        TryDie();
    }

    public void Kill(GameObject source = null)
    {
        if (isDead)
            return;

        float before = CurrentHealth;
        CurrentHealth = 0f;

        float damageTaken = before;
        if (damageTaken > 0f)
        {
            OnDamaged?.Invoke(damageTaken, source);
        }

        Die();
    }

    private void TryDie()
    {
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        OnDie?.Invoke();
    }
}