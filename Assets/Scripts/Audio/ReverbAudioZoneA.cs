using UnityEngine;
using FMOD;
using FMODUnity;

//This script handles a reverb snapshot within a collider. The designated sound busses are setup in unity.
//upon the player object entering the collider, the snapshot gets triggered and on exit the snapshot fades out. 
public class ReverbAudioZoneA : MonoBehaviour
{
    FMOD.Studio.EventInstance reverbInstance;
    [SerializeField] private GameObject playerObject;

    private void Start()
    {
        reverbInstance = RuntimeManager.CreateInstance("snapshot:/ReverbZone");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerObject)
        {
            UnityEngine.Debug.Log("Player entered reverb zone A");
            reverbInstance.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerObject)
        {
            UnityEngine.Debug.Log("Player exited reverb zone A");
            reverbInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        reverbInstance.release();
    }
}
