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

    private void Start()
    {
        FocusController.instance.sceneType = currentScene;
    }
}
