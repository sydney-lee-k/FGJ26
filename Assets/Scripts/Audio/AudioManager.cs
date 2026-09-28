using FMODUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;
using Random = UnityEngine.Random;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (!transform.parent) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Trying to create a duplicate Audio Manager on " + gameObject.name);
            Destroy(gameObject);
            return;
        }
    }

    public void playOneShot(EventReference sound, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(sound, pos);
    }

}
