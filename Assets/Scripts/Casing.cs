using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Casing : MonoBehaviour
{
    [SerializeField] private float activeTime = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(DisablePhysicsAfterTime());
    }

    private IEnumerator DisablePhysicsAfterTime()
    {
        yield return new WaitForSeconds(activeTime);
        DisablePhysics();
    }

    private void DisablePhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}