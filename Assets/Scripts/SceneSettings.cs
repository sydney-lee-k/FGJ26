using System;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    public enum SceneType
    {
        Nonplayable,
        Combat,
        Noncombat
    }
    
    public SceneType currentScene = SceneType.Combat;
    private bool initialized;

    private void Start()
    {
        initialized = true;
        Debug.Log("Ran");
        //FocusController.instance.sceneType = currentScene;
    }

    private void LateUpdate()
    {
        if (initialized)
        {
            Debug.Log("Ran");
            initialized = false;
            FocusController.instance.sceneType = currentScene;
        }
    }
}
