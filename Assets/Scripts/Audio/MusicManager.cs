using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MusicManager : MonoBehaviour
{

    public static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (!transform.parent) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Trying to create a duplicate Music Manager on " + gameObject.name);
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

    public void BeginPlay(string key)
    {
        foreach (var lib in activeLibraries)
        {
            var e = lib.Get(key);
            if (!e.IsNull) {/*play func here*/}
        }
    }

    public void OnCombatBegin()
    {
        //this method will change the music to a more intense version/different one entirely after combat begins in a level
        //combat begins on: ???. Do "stealth" kills count?
    }


    public void EndPlay(string key)
    {

    }
}
