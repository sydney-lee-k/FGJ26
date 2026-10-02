using System;
using UnityEngine;

public class Tracer : MonoBehaviour, IPoolable
{
    private LineRenderer lineRenderer;
    private Material lineMaterial;

    [Header("Variables")]
    [SerializeField] private float fadeDuration = 1f;

    private Vector3 targetPosition;
    private float dissolve;
    private bool initialized;
    private static readonly int DissolveID = Shader.PropertyToID("_Dissolve");

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineMaterial = new Material(lineRenderer.sharedMaterial);
        lineRenderer.material = lineMaterial;
    }

    void Update()
    {
        if (!initialized)
            return;

        dissolve += Time.deltaTime / fadeDuration;

        if (dissolve >= 1)
        {
            PoolManager.Instance.Despawn(gameObject);
            return;
        }

        lineMaterial.SetFloat(DissolveID, dissolve);
    }

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
        initialized = true;
    }

    public void OnPoolSpawn()
    {
        dissolve = 0;
        initialized = false;
        lineMaterial.SetFloat(DissolveID, 0);
    }

    public void OnPoolDespawn()
    {
        initialized = false;
    }
}