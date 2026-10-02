using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnPoolSpawn();
    void OnPoolDespawn();
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Excess pool manager:" + gameObject.name);
            return;
        }
        else Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("PoolManager: Tried to spawn a null prefab.");
            return null;
        }

        if (!pools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools.Add(prefab, pool);
        }

        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = CreateInstance(prefab);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        foreach (IPoolable poolable in obj.GetComponentsInChildren<IPoolable>(true))
        {
            poolable.OnPoolSpawn();
        }

        obj.transform.parent = parent;
        return obj;
    }

    public GameObject AddToPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("PoolManager: Tried to add a null prefab to the pool.");
            return null;
        }

        if (!pools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools.Add(prefab, pool);
        }

        GameObject obj = CreateInstance(prefab);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.parent = parent;
        obj.SetActive(false);

        pool.Enqueue(obj);

        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null)
            return;

        if (!instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            Debug.LogWarning($"PoolManager: Tried to despawn an object that wasn't spawned by the PoolManager: {obj.name}");

            return;
        }

        foreach (IPoolable poolable in obj.GetComponentsInChildren<IPoolable>(true))
        {
            poolable.OnPoolDespawn();
        }

        obj.SetActive(false);

        pools[prefab].Enqueue(obj);
    }
    
    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);

        instanceToPrefab.Add(obj, prefab);

        obj.SetActive(false);

        return obj;
    }
}