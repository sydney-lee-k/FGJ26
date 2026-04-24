using System.Collections.Generic;
using UnityEngine;

public class ActorsManager : MonoBehaviour
{
    public static ActorsManager Instance { get; private set; }

    private readonly HashSet<Actor> actors = new();
    public IReadOnlyCollection<Actor> Actors => actors;

    public Actor Player { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
    }

    public void UnregisterActor(Actor actor)
    {
        actors.Remove(actor);
    }

    public void SetPlayer(Actor player)
    {
        Player = player;
    }
}