using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject squibPrefab;

    private static BloodSpawner instance;

    void Awake()
    {
        instance = this;
    }

    public static void HandleBlood(Vector3 pos)
    {
        Quaternion lookRotation = Quaternion.LookRotation(instance.target.position - instance.transform.position);
        pos.y += 1;
        GameObject tempGO = Instantiate(instance.squibPrefab, pos, lookRotation);
        
        Destroy(tempGO, 2f);
    }
}
