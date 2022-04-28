using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    TransitionState,
    EventState,
    AnswerState,
}

public sealed class GameController : MonoBehaviour
{
    public static GameController Instance;


    public float HabitationMeter { get; private set; }
    public float FoodMeter { get; private set; }
    public float JoyMeter { get; private set; }

    public List<RandomEvents> EventPool = new List<RandomEvents>();

    private List<RandomEvents> _eventsList = new List<RandomEvents>();

    private GameState _gameState = GameState.TransitionState;

    [SerializeField] private float _habitationMax = 10.0f;
    [SerializeField] private float _foodMax = 10.0f;
    [SerializeField] private float _joyMax = 10.0f;
    [SerializeField] private float _joyLoseRatio = 0.0f;
    [SerializeField] private float _foodGainRatio = 0.0f;
    [SerializeField] private float _HabitationGainRatio = 0.0f;

    [SerializeField] private Slider _sliderFood;
    [SerializeField] private Slider _sliderHabitation;
    [SerializeField] private Slider _sliderJoy;

    [SerializeField] private float _advancementFactor = 0.2f;

    private float _advancementValue = 1.0f;
    private float _innerTimer;
    private bool _isFoodDepleting = false;
    private bool _isHabitationDepleting = false;

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
        HabitationMeter = _habitationMax * 0.75f;
        FoodMeter = _foodMax * 0.75f;
        JoyMeter = _joyMax * 0.75f;

        //TODO : Set the Melody of the events dynamically here

        for (int i = 0; i < EventPool.Count; i++)
        {
            if (EventPool[i].EventType == EventType.Event)
            {
                switch (EventPool[i].IncantationType)
                {
                    case IncantationType.Sun:
                    {
                        break;
                    }
                    case IncantationType.Health:
                    {
                        break;
                    }
                    case IncantationType.Love:
                    {
                        break;
                    }
                    case IncantationType.Rain:
                    {
                        break;
                    }
                    default:
                    {
                        Debug.Log("Forgot to set Incantation type of the Event : " + EventPool[i].EventName);
                        break;
                    }
                }
            }
            else
            {

            }
        }
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

        float precedentFoodMeter = FoodMeter;
        float precedentHabitationMeter = FoodMeter;
        
        for (int i = 0; i < _eventsList.Count; ++i)
        {
            FoodMeter -= _eventsList[i].FoodValue * Time.deltaTime;
            HabitationMeter -= _eventsList[i].HabitationValue * Time.deltaTime;
        }

        if (precedentFoodMeter != FoodMeter)
            _isFoodDepleting = true;
        else
            _isFoodDepleting = false;

        if (precedentHabitationMeter != HabitationMeter)
            _isHabitationDepleting = true;
        else
            _isHabitationDepleting = false;

        if (!_isFoodDepleting)
        {
            FoodMeter += _foodGainRatio * Time.deltaTime;
        }

        if (!_isHabitationDepleting)
        {
            HabitationMeter += _HabitationGainRatio * Time.deltaTime;
        }

        JoyMeter -= _joyLoseRatio * Time.deltaTime;

        _sliderFood.value = FoodMeter / _foodMax;
        _sliderHabitation.value = HabitationMeter / _habitationMax;
        _sliderJoy.value = JoyMeter / _joyMax;
    }

    private void UpdateGameState()
    {
        switch (_gameState)
        {
            case GameState.TransitionState:
            {
                Debug.Log("Entering Transition state");
                if(_advancementValue < 2)
                    _advancementValue += _advancementFactor;
                _innerTimer = 5.0f;
                break;
            }
            case GameState.EventState:
            {
                Debug.Log("Entering Event state");

                for (int i = 0; i < (int)_advancementValue; ++i)
                {
                    int rand = Random.Range(0, EventPool.Count);
                    _eventsList.Add(EventPool[rand]);
                    Debug.Log("Between " + EventPool.Count + " events, i chose the number " + rand + " " + EventPool[rand].EventName + " the event melody is " );
                }
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
