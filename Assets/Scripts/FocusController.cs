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
    private float focusGrowthRate = 1f;

    private void Awake()
    {
        instance = this;
        input = InputReader.instance;
        focusMaterials = Resources.LoadAll<Material>("FocusMaterials");
    }

    private void Start()
    {
        initialized = true;
        input.OnFocusDown += HandleFocus;
    }

    private void Update()
    {
        currentFocus = Mathf.Clamp01(focusing ? currentFocus+Time.deltaTime * focusGrowthRate : currentFocus-Time.deltaTime * focusGrowthRate);
        
        if (sceneType == SceneSettings.SceneType.Combat)
        {
            foreach (var mat in focusMaterials)
            {
                mat.SetFloat("_Focus", currentFocus);
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
