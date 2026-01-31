using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [SerializeField] private AudioSource audioMusicSource;
    [SerializeField] private AudioClip[] audioClips;

    public void PlaySound(int clipID, AudioSource source)
    {
        if (audioClips[clipID] == null || source == null)
        {
            return;
        }
        else
        {
            //Creates minor randomization to sounds before playing them
            float x = Random.Range(0.90f, 1.10f);
            source.pitch = x;
            source.PlayOneShot(audioClips[clipID]);
        }
    }

    public void PlayMusic()
    {
        audioMusicSource.Play();
    }
}
