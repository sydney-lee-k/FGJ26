using UnityEngine;

//This can be attached to animation events for easy access to sounds. 
//Add an event to the animation timeline at the desired spot and name the function and pass the path key as a string
public class FMODPlayer : MonoBehaviour
{
    void PlaySound(string key)
    {
        AudioManager.instance.PlaySound(key, GetComponent<Transform>().position);
    }
}