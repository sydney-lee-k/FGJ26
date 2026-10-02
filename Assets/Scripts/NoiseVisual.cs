using System;
using UnityEngine;

public class NoiseVisual : MonoBehaviour, IPoolable
{
    public float radius;
    private ParticleSystem pSystem;
    private ParticleSystem.MainModule psMain;
    
    private bool initialized = false;

    private void Awake()
    {
        OnPoolSpawn();
    }

    public void OnPoolSpawn()
    {
        initialized = true;
    }

    public void OnPoolDespawn()
    {
    }

    private void LateUpdate()
    {
        if (initialized)
        {
            if (!pSystem)
            {
                pSystem = GetComponent<ParticleSystem>();
                psMain = pSystem.main;
            }

            psMain.startSize = Mathf.Max(radius);
            pSystem.Play();
            initialized = false;
        }
    }

    public void OnParticleSystemStopped()
    {
        PoolManager.Instance.Despawn(gameObject);
    }
}
