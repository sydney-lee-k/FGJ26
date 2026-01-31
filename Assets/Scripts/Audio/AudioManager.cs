using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;


public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
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

    public void PlaySoundSplatter(Vector3 pos)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;
        AudioSource source = tempGO.AddComponent<AudioSource>();
        AudioClip clip = audioClips_Splatters[Random.Range(0, audioClips_Splatters.Length)];
        source.clip = clip;
        source.volume = splattersVolume;
        source.pitch = Random.Range(1 - soundVar, 1 + soundVar);
        source.spatialBlend = 1;
        source.Play();
        Destroy(tempGO, clip.length);
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
}
