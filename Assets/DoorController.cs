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

    public void Interact()
    {
        isOpen = !isOpen;
        Vector3 heading = player.position - transform.position;
        float dot = Vector3.Dot(heading, player.forward);
        anim.SetBool("Forward", dot > 0);
        
        if(isOpen) Open();
        else Close();
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
