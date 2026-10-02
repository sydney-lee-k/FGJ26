using System;
using Unity.VisualScripting;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    private InputReader input;
    public static FocusController instance;
    public SceneSettings.SceneType sceneType; //When this is assigned we can assume you are in a gameplay scene.
    public bool focusing;
    private bool initialized;

    private Material[] focusMaterials;
    private float currentFocus;
    private float focusGrowthRate;

    private void Start()
    {
        input = InputReader.instance;
        focusMaterials = Resources.LoadAll<Material>("FocusMaterials");
        
        initialized = true;
        input.OnFocusDown += HandleFocus;
    }

    private void Update()
    {
        if (sceneType == SceneSettings.SceneType.Combat)
        {
            Debug.Log("Test");
            foreach (var mat in focusMaterials)
            {
                mat.SetFloat("_Focus", focusing ? 1 : 0);
            }
        }
        else
        {
            return;
        }
    }


    private void HandleFocus(bool focus)
    {
        if (sceneType != SceneSettings.SceneType.Combat)
        {
            focusing = false;
            return;
        }
        focusing = focus;
        Debug.Log(focusing);
    }
    
    private void OnEnable()
    {
        if(initialized) input.OnFocusDown += HandleFocus;
    }

    private void OnDisable()
    {
        input.OnFocusDown -= HandleFocus;
    }
}
