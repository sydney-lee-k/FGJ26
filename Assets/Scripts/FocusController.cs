using System;
using Unity.VisualScripting;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    private InputReader input;
    public static FocusController instance;

    public float unfocusedVisibilityDistance;
    
    [NonSerialized] public float currentFocus;
    [NonSerialized] public bool focusing;
    [NonSerialized] public SceneSettings.SceneType sceneType; //When this is assigned we can assume you are in a gameplay scene.
    private bool initialized;
    private float focusGrowthRate = 1f;
    [NonSerialized] public FieldOfView fov;

    private void Awake()
    {
        instance = this;
        input = InputReader.instance;
        fov = FindAnyObjectByType<FieldOfView>();
    }

    private void Start()
    {
        initialized = true;
        input.OnFocusDown += HandleFocus;
    }

    private void Update()
    {
        currentFocus = Mathf.Clamp01(focusing ? currentFocus+Time.deltaTime * focusGrowthRate : currentFocus-Time.deltaTime * focusGrowthRate);
    }

    public float CalculateVisibility(Transform self, Transform target)
    {
        float a = (Mathf.Clamp(Vector3.Distance(self.position, target.position), 0f, unfocusedVisibilityDistance) / unfocusedVisibilityDistance);
        a = 1 - a * a * a; //Power of 3, but without Mathf.Pow to optimize
        return a;
    }

    private void HandleFocus(bool focus)
    {
        if (sceneType != SceneSettings.SceneType.Combat)
        {
            focusing = false;
            return;
        }
        focusing = focus;
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
