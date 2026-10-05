using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Event Library")]
public class AudioEventLibrary : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string eventKey;
        public EventReference eventRef;
    }

    [BankRef] public List<string> banks = new();

    public List<Entry> events = new();
    Dictionary<string, EventReference> eventRefs;


    public EventReference Get(string key)
    {
        if (eventRefs == null)
        {
            eventRefs = new();
            foreach (var e in events) eventRefs[e.eventKey] = e.eventRef;
        }
        return eventRefs.TryGetValue(key, out var r) ? r : default;
    }
}