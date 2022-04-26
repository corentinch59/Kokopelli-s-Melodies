using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RandomEvents", order = 1)]
public class RandomEvents : ScriptableObject
{
    public float EventDuration;
    public float HabitationValue;
    public float FoodValue;
    public float JoyValue;
    public string EventName;
}
