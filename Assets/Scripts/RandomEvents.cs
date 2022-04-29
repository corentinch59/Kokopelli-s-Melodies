using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    Quest,
    Event
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RandomEvents", order = 1)]
public class RandomEvents : ScriptableObject
{
    public EventType EventType;
    public IncantationType IncantationType;
    public float HabitationValue;
    public float FoodValue;
    public string EventName;
    public Melody EventMelody;
}
