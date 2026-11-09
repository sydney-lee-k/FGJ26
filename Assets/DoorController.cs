using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen;
    [SerializeField] private Animator anim;

    private Transform player;

    public List<GameObject> inDoorway;
    private float angleToOpen = 15;
    private void Start()
    {
        if(!anim) anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim.Play("DoorClosedForward", 0, 1);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8 && !inDoorway.Contains(other.gameObject)) inDoorway.Add(other.gameObject);
        if (!isOpen && other.gameObject.layer == 8) //Enemy layer
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();

            float angle = Vector3.Angle(transform.forward, enemyController.transform.forward);
            Debug.Log(angle);
            if (angle < angleToOpen || angle > 360-angleToOpen || angle > 180 - angleToOpen && angle < 180+angleToOpen)
            {
                if(enemyController && enemyController.canOpenDoors) EnemyOpen(other.transform, (angle < angleToOpen || angle > 360-angleToOpen));
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        inDoorway.Remove(other.gameObject);
        if (isOpen && inDoorway.Count == 0 && other.gameObject.layer == 8) //Enemy layer
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if(enemyController && enemyController.canOpenDoors) EnemyClose(enemyController);
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        Vector3 heading = player.position - transform.position;
        float dot = Vector3.Dot(heading.normalized, transform.forward);
        anim.SetBool("Forward", dot < 0);
        Debug.Log(dot);
        
        if(isOpen) Open();
        else Close();
    }
    
    public void EnemyOpen(Transform enemy, bool forward)
    {
        isOpen = true;
        anim.SetBool("Forward", forward);
        Open();
    }
    
    public void EnemyClose(EnemyController enemyController)
    {
        if (enemyController.state == EnemyController.EnemyState.Patrol || enemyController.state == EnemyController.EnemyState.Scout)
        {
            isOpen = false;
            Close();
        }
    }
    
    public void Open()
    {
        anim.SetBool("Open", isOpen);
    }
    
    public void Close()
    {
        anim.SetBool("Open", isOpen);
    }
}
