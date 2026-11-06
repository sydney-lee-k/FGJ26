using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen;
    [SerializeField] private Animator anim;

    private void Start()
    {
        if(!anim) anim = GetComponent<Animator>();
        anim.Play("DoorClosed", 0, 1);
    }

    public void Interact()
    {
        isOpen = !isOpen;
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
