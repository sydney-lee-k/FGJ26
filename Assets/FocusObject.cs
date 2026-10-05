using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class FocusObject : MonoBehaviour
{
    private Transform player;
    private FocusController focusController;
    
    private ParticleSystem pSystem;
    private List<Material> focusMaterials = new List<Material>();
    private List<Renderer> focusRenderers = new List<Renderer>();

    private float currentFocus;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        focusController = FocusController.instance;
        if (!focusController) return;
        
        pSystem = GetComponent<ParticleSystem>();
        if (pSystem)
        {
            //Create an instance of the particle system material.
            focusRenderers.Add(pSystem.GetComponent<Renderer>());
            focusMaterials.Add(new Material(focusRenderers[0].material));
        }
        
        
        for (int i = 0; i < focusMaterials.Count; i++)
        {
            focusRenderers[i].material = focusMaterials[i];
        }
    }

    private void Update()
    {
        if (!focusController) return;
        
        //Gets a 1 - 0 range for visibility based on distance. Reach set in FocusController.
        float distanceVisibility = focusController.CalculateVisibility(transform, player);
            
        currentFocus = Mathf.Max(distanceVisibility, focusController.currentFocus);

        foreach (var focusMat in focusMaterials)
        {
            focusMat.SetFloat("_Focus", currentFocus);
        }
    }
}
