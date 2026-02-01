using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BloodTrigger : MonoBehaviour
{
    [SerializeField] private VisualEffect bloodSplatter;

    [SerializeField] private GameObject prefab;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rayCastDistance;
    private Vector3 randDirection;
    private RaycastHit hit;


    private void Start()
    {
        PlayParticle();
    }


    private void PlayParticle()
    {
        //Raycast();
        bloodSplatter.Play();
    }

    /*
    private void Raycast()
    {
        randDirection = Random.insideUnitSphere.normalized;

        if (Physics.Raycast(transform.position, randDirection, out hit, rayCastDistance, collisionLayer))
        {
            StartCoroutine(DelaySpawnCoroutine(GetTimeToArrive(Vector3.Distance(transform.position, hit.point), projectileSpeed), 
                hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal)));
        }
    }

    IEnumerator DelaySpawnCoroutine(float timeToArrive, Vector3 pos, Quaternion rot)
    {
        Debug.Log("Running coroutine");
        yield return new WaitForSeconds(timeToArrive);
        Instantiate(prefab, pos, rot);
    }

    private float GetTimeToArrive(float distance, float speed)
    {
        return distance / speed;
    }*/
}
 