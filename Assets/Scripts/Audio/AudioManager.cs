using FMODUnity;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

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

    readonly List<AudioEventLibrary> activeLibraries = new();

    // add the necessary library for current scene into activeLibraries list. Called in SceneAudioLoader.cs at the start of a scene
    public void RegisterLibrary(AudioEventLibrary lib)
    {
        if (!activeLibraries.Contains(lib)) activeLibraries.Add(lib);
    }

    public void UnregisterLibrary(AudioEventLibrary lib) => activeLibraries.Remove(lib);


    //find the event in the currently registered library and play sound (without parameters)
    public void PlaySound(string key, Vector3 pos = default)
    {
        foreach (var lib in activeLibraries)
        {
            var e = lib.Get(key);
            if (!e.IsNull) { RuntimeManager.PlayOneShot(e, pos); return; }
        }
        Debug.LogWarning($"Audio event not found: {key}");
    }

    //Find the event in the currently registered library
    //Create EventInstance and set attributes and parameters
    //Play event
    public void PlayParamSound(string key, Vector3 pos = default, string paramName = null, int paramValue = 0)
    {
        foreach (var lib in activeLibraries)
        {
            var e = lib.Get(key);
            if (!e.IsNull) 
            {
                FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(e);
                eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos)); //Set position for 3D audio

                //Set the parameter value in the named parameter. values start from 0. Check audio documentation for the value specifics
                eventInstance.setParameterByName(paramName, paramValue); 
                eventInstance.start(); //Play event
                eventInstance.release(); //Release event to avoid eventual memory leaks
            }
        }
        Debug.LogWarning($"Audio event not found: {key}");
    }

    public void PlaySnapshot(string key)
    {
        Debug.Log("Changed snapshot");
        foreach (var lib in activeLibraries)
        {
            var e = lib.Get(key);
            if (!e.IsNull)
            {
                FMOD.Studio.EventInstance snapshot = FMODUnity.RuntimeManager.CreateInstance(e);
                Debug.Log("created instance");
                snapshot.start();
                Debug.Log("started instance");
                snapshot.setParameterByName("Intensity", 1.0f);
            }
        }
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}
