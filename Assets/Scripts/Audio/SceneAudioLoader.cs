using System.Collections;
using FMODUnity;
using UnityEngine;

public class SceneAudioLoader : MonoBehaviour
{
    [SerializeField] AudioEventLibrary library;

    public bool isReady { get; private set; }

    //Prepare audiomanager with banks and libraries for loaded scene
    //PROBLEM: some gameobjects may try to play audio before banks and libraries are loaded, leading to "Event Not Found" errors. This needs to happen before anything else audio related.
    void Awake()
    {
        foreach (var bank in library.banks)
            RuntimeManager.LoadBank(bank, true); //load sample data

        //wait for banks and sample data to finish loading
        //while (!RuntimeManager.HaveAllBanksLoaded || RuntimeManager.AnySampleDataLoading())
        //    yield return null;
        RuntimeManager.WaitForAllSampleLoading();

        AudioManager.instance.RegisterLibrary(library);
        isReady = true;
        //AudioManager.instance.PlaySound("DebugMusic");
    }

    //Unregister and unload event library and banks to clear audiomanager for new sets on next scene load
    void OnDestroy()
    {
        if (library == null) return;

        if (AudioManager.instance != null)
            AudioManager.instance.UnregisterLibrary(library);

        foreach (var bank in library.banks)
            RuntimeManager.UnloadBank(bank);
    }
}