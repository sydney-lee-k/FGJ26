using UnityEngine;
using Random = UnityEngine.Random;

public class NPCcontroller : MonoBehaviour
{
    [SerializeField] private GameObject[] maskObjects;
    [SerializeField] private Material[] materials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Health health;

    void Start()
    {
        SetRigidbodyState(true);
        SetColliderState(false);

        health.OnDie += Die;

        GameObject selectedMask = maskObjects[Random.Range(0, maskObjects.Length)];
        Material selectedMaterial = materials[Random.Range(0, materials.Length)];
        

    }

    // Update is called once per frame
    public void Die()
    {
        AudioManager.PlaySoundAt(SoundType.DEATH, transform.position);
        BloodSpawner.HandleBlood(transform.position);
        GetComponent<Animator>().enabled = false;
        SetRigidbodyState(false);
        SetColliderState(true);

    }

    private void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = !state;
    }

    private void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }
}
