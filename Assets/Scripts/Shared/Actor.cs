using System;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public enum Affiliation
    {
        Neutral,
        Player,
        Enemy
    }

    [Tooltip("Represents the team of the actor. Actors of the same affiliation are friendly to each other")]
    public Affiliation affiliation;

    [Tooltip("Represents where other actors will aim when they attack this actor")]
    [SerializeField] private Transform aimPoint;
    public Transform AimPoint => aimPoint != null ? aimPoint : transform;

    private ActorsManager actorsManager;

    private void Start()
    {
        actorsManager = ActorsManager.Instance;
    }

    private void OnEnable()
    {
        if(actorsManager) actorsManager.RegisterActor(this);
    }

    private void OnDisable()
    {
        actorsManager.UnregisterActor(this);
    }
}