using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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
    private static AudioManager instance;

    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioSource audioSourceSFX;
    [SerializeField] private AudioSource audioSourceMusic;

    [SerializeField] private AudioMixerGroup sfxMixer;


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

    public static void PlayMusic()
    {
        instance.audioSourceMusic.Play();
    }

    //Plays sound by SoundType
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;                                  //List of available clips in given SoundType
        AudioClip clip = clips[Random.Range(0, clips.Length)];                                      //Randomize clip
        instance.audioSourceSFX.pitch = Random.Range(1 - instance.soundVar, 1 + instance.soundVar);    //Randomize pitch
        instance.audioSourceSFX.PlayOneShot(clip, volume);                                             //Play clip
    }

    //Plays sound by SoundType through a temporary GameObject at a given location
    public static void PlaySoundAt(SoundType sound, Vector3 pos, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;                  //List of available clips in given SoundType
        AudioClip clip = clips[Random.Range(0, clips.Length)];                      //Randomize clip

        GameObject tempGO = new GameObject("TempAudio");                            //Creates temporary GameObject
        tempGO.transform.position = pos;                                            //Places temporary GameObject at given position
        AudioSource source = tempGO.AddComponent<AudioSource>();                    //Adds AudioSource component to temporary GameObject

        source.pitch = Random.Range(1 - instance.soundVar, 1 + instance.soundVar);
        source.outputAudioMixerGroup = instance.sfxMixer;                                      //Randomize pitch
        source.spatialBlend = 1;                                                    //Enable Spatial Blending
        source.PlayOneShot(clip, volume);                                           //Play clip

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
