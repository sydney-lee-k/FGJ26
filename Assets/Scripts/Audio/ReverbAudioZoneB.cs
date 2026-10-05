using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using FMOD;

//This script triggers a distance affected reverb snapshot. The designated sound busses are setup in unity.
//All handling of the distance parameter is handled by fmod based on the listener object and its attenuation object (main camera & player)
public class ReverbAudioZoneB : MonoBehaviour
{
    EventInstance reverbInstance;


    private void Start()
    {
        reverbInstance = RuntimeManager.CreateInstance("snapshot:/Reverbparam");    
        RuntimeManager.AttachInstanceToGameObject(reverbInstance, gameObject, GetComponent<Rigidbody>());
        reverbInstance.start();
        reverbInstance.release();
    }
        
    private void OnDestroy()
    {
        reverbInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
