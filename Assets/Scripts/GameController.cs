using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    TransitionState,
    EventState,
    AnswerState,
}

public sealed class GameController : MonoBehaviour
{
    public static GameController Instance;

    public float HabitationMeter { get; private set; } = 10.0f;
    public float FoodMeter { get; private set; } = 10.0f;
    public float JoyMeter { get; private set; } = 10.0f;

    public List<RandomEvents> PossibleEventsList = new List<RandomEvents>();
    public List<RandomEvents> EventsList = new List<RandomEvents>();

    private GameState GameState = GameState.TransitionState;

    private float _innerTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        switch (GameState)
        {
            case GameState.TransitionState:
                break;
            case GameState.EventState:
                break;
            case GameState.AnswerState:
                break;
            default:
                Debug.Log("Default Resolution of the game state");
                break;
        }

        _innerTimer += Time.deltaTime;
    }
}
