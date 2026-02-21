using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

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
 