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

    public List<RandomEvents> EventPool = new List<RandomEvents>();

    private List<RandomEvents> _eventsList = new List<RandomEvents>();
    private GameState _gameState = GameState.TransitionState;

    private float _innerTimer;

    //TODO : Multiple events possible being chosen in the event pool
    //TODO : Incorporate in the EventState (exponentially increases the number of event ?)

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
        if (_innerTimer < 0)
        {
            ++_gameState;
            if (_gameState > (GameState)2)
                _gameState = GameState.TransitionState;
            UpdateGameState();
        }
        
        _innerTimer -= Time.deltaTime;
    }

    private void UpdateGameState()
    {
        switch (_gameState)
        {
            case GameState.TransitionState:
            {
                Debug.Log("Entering Transition state");
                _innerTimer = 5.0f;
                break;
            }
            case GameState.EventState:
            {
                Debug.Log("Entering Event state");

                int rand = Random.Range(0, EventPool.Count);
                _eventsList.Add(EventPool[rand]);
                Debug.Log("Between " + EventPool.Count + " events, i chose the number " + rand + " " + EventPool[rand].EventName);
                break;
            }
            case GameState.AnswerState:
            {
                Debug.Log("Entering Answer state");
                _innerTimer = 5.0f;
                break;
            }
            default:
            {
                Debug.Log("Default Resolution of the game state");

                break;
            }
        }
    }
}
