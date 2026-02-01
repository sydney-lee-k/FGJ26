using System;
using UnityEngine;
using UnityEngine.VFX;

public class BloodTrigger : MonoBehaviour
{
    [SerializeField] private VisualEffect bloodSplatter;
    
    private void Start()
    {
        PlayParticle();
    }


    private void PlayParticle()
    {
        bloodSplatter.Play();
    }
}
