using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrePooling : MonoBehaviour
{
    [SerializeField] private PrePoolable[] prePoolables;
    private PoolManager poolManager;
    private void Start()
    {
        poolManager = PoolManager.Instance;

        StartCoroutine(PrePool());
    }

    IEnumerator PrePool()
    {
        const float maxMilliseconds = 2f;
        foreach (var poolable in prePoolables)
        {
            for (int i = 0; i < poolable.count; i++)
            {
                float startTime = Time.realtimeSinceStartup;
                poolManager.AddToPool(poolable.prefab, new Vector3(0, -100, 0), Quaternion.identity, poolManager.transform);

                float elapsedMilliseconds = (Time.realtimeSinceStartup - startTime) * 1000f;

                if (elapsedMilliseconds >= maxMilliseconds) yield return null;
            }
        }
    }
}

[Serializable]
class PrePoolable
{
    public GameObject prefab;
    public float count;
}