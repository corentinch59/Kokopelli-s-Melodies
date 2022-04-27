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
    [SerializeField] private EventType _eventType;

    public float HabitationValue;
    public float FoodValue;
    public string EventName;
    public RandomEvents()
    {

    }
}
