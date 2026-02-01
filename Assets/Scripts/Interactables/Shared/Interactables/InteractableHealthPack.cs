using UnityEngine;
using System.Collections.Generic;

public class InteractableHealthPack : MonoBehaviour
{
    [SerializeField] private Health health;

    public void OnHealthPackInteract()
    {
        Debug.Log("Healing player");
        //Play animation
        health.Heal(3);
    }
}
