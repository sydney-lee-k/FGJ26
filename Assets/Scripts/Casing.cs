using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Casing : MonoBehaviour, IPoolable
{
    [SerializeField] private float activeTime = 10f;
    private float remainingActiveTime;

    private bool inactive;
    private Collider coll;
    private Rigidbody rb;
    
    private void Update()
    {
        if (inactive) return;

        //After active time disable physics. If active time has been below 0 for more than a minute and its still somehow moving then just freeze it wherever it is.
        if (remainingActiveTime <= -60 || (remainingActiveTime <= 0 && rb.linearVelocity.magnitude <= 0.05f && rb.angularVelocity.magnitude <= 0.05f))
        {
            DisablePhysics();
            inactive = true;
        }
        else
        {
            remainingActiveTime -= Time.deltaTime;
        }
    }

    private void DisablePhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.detectCollisions = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        coll.enabled = false;
    }

    public void OnPoolSpawn()
    {
        remainingActiveTime = activeTime;
        inactive = false;
        if(!rb) rb = GetComponent<Rigidbody>();
        if(!coll) coll = GetComponent<Collider>();
        coll.enabled = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    public void OnPoolDespawn()
    {
        
    }
}