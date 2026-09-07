using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    public static PatrolController Instance;
    public List<PatrolRoute> routes = new List<PatrolRoute>();
    private void Awake()
    {
        //Patrol Controller swaps if you swap a scene that has a new Patrol Controller.
        Instance = this;
    }
}
