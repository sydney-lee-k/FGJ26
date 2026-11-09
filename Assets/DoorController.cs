using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen;
    [SerializeField] private Animator anim;

    private Transform player;

    private void Start()
    {
        if(!anim) anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim.Play("DoorClosedForward", 0, 1);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isOpen && other.gameObject.layer == 8) //Enemy layer
        {
            EnemyOpen(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isOpen && other.gameObject.layer == 8) //Enemy layer
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if(enemyController) EnemyClose(enemyController);
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
    
    public void EnemyOpen(Transform enemy)
    {
        Vector3 heading = enemy.position - transform.position;
        float dot = Vector3.Dot(heading.normalized, enemy.forward);
        Debug.Log(dot);
        if (dot > 0.3f || dot < -0.3f)
        {
            isOpen = true;
            anim.SetBool("Forward", dot < 0);
            Open();
        }
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
