using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
using Random = UnityEngine.Random;


public enum SoundType
{
    CLICK,
    FOOTSTEP,
    MASK,
    DEATH,
    PISTOL,
    SHOTGUN,
    RIFLE
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    /*
    [Range(0, 1f)] public float soundVar;

    [SerializeField] [Range(0, 1)] private float generalVolume;
    [SerializeField] [Range(0, 1)] private float stepsVolume;
    [SerializeField] [Range(0, 1)] private float splattersVolume;
    [SerializeField] [Range(0, 1)] private float gunshotsVolume;

    [SerializeField] private AudioSource audioMusicSource;

    [SerializeField] private AudioClip[] audioClips_general;
    [SerializeField] private AudioClip[] audioClips_Steps;

    [SerializeField] private AudioClip[] audioClips_Pistol;
    [SerializeField] private AudioClip[] audioClips_Shotgun;
    [SerializeField] private AudioClip[] audioClips_Rifle;

    [SerializeField] private AudioClip[] audioClips_Splatters;
    

    public void PlaySoundGeneral(int clipID, AudioSource source)
    {
        if (audioClips_general[clipID] == null || source == null)
        {
            return;
        }
        else
        {
            //Creates minor randomization to sounds before playing them
            float x = Random.Range(1 - soundVar, 1 + soundVar);
            source.volume = generalVolume;
            source.pitch = x;
            source.PlayOneShot(audioClips_general[clipID]);
        }
    }

    public void PlaySoundStep(AudioSource source)
    {
        source.volume = stepsVolume;
        source.pitch = Random.Range(1 - soundVar, 1 + soundVar);
        source.PlayOneShot(audioClips_Pistol[Random.Range(0, audioClips_Steps.Length)]);
    }

    

    public void PlaySoundGunshot(int gunID, AudioSource source)
    {
        source.volume = gunshotsVolume;
        switch (gunID)
        {
            case 0: //PISTOL               
                    source.pitch = Random.Range(1 - soundVar, 1 + soundVar);
                    source.PlayOneShot(audioClips_Pistol[Random.Range(0, audioClips_Pistol.Length)]);               
                break;
            case 1: //SHOTGUN
                    source.pitch = Random.Range(1 - soundVar, 1 + soundVar); ;
                    source.PlayOneShot(audioClips_Shotgun[Random.Range(0, audioClips_Shotgun.Length)]);
                break;
            case 2: //RIFLE
                    source.pitch = Random.Range(1 - soundVar, 1 + soundVar); ;
                    source.PlayOneShot(audioClips_Rifle[Random.Range(0, audioClips_Rifle.Length)]);
                break;
        }
    }
    public void PlayMusic()
    {
        audioMusicSource.Play();
    }
    public void StopMusic()
    {
        audioMusicSource.Stop();
    }
    */


    private static AudioManager instance;

    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] [Range (0, 1f)] private float soundVar;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //Plays sound by SoundType
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;                                  //List of available clips in SoundType
        AudioClip clip = clips[Random.Range(0, clips.Length)];                                      //Randomize clip
        instance.audioSource.pitch = Random.Range(1 - instance.soundVar, 1 + instance.soundVar);    //Randomize pitch
        instance.audioSource.PlayOneShot(clip, volume);                                             //Play clip
    }

    //Plays sound by SoundType through a temporary GameObject at a given location
    public static void PlaySoundAt(SoundType sound, Vector3 pos, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;                  //List of available clips in SoundType
        AudioClip clip = clips[Random.Range(0, clips.Length)];                      //Randomize clip

        GameObject tempGO = new GameObject("TempAudio");                            //Creates temporary GameObject
        tempGO.transform.position = pos;                                            //Places temporary GameObject at given position
        AudioSource source = tempGO.AddComponent<AudioSource>();                    //Adds AudioSource component to temporary GameObject

        source.pitch = Random.Range(1 - instance.soundVar, 1 + instance.soundVar);  //Randomize pitch
        source.spatialBlend = 1;                                                    //Enable Spatial Blending
        source.Play();                                                              //Play clip

        Destroy(tempGO, clip.length);                                               //Destroys temporary GameObject after clip has played
    }
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [SerializeField] private string name;
    [SerializeField] private AudioClip[] sounds;
}
