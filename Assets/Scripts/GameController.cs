using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PlayState
{
    TransitionState,
    EventState,
    AnswerState,
}

public enum GameState
{
    Play,
    Pause,
    Event,
    Quest
}

public sealed class GameController : MonoBehaviour
{
    public static GameController Instance;


    public float HabitationMeter { get; private set; }
    public float FoodMeter { get; private set; }
    public float JoyMeter { get; private set; }

    [Header("Lists of Events")]
    public List<RandomEvents> EventPool = new List<RandomEvents>();
    public List<RandomEvents> QuestPool = new List<RandomEvents>();

    private List<RandomEvents> _eventsList = new List<RandomEvents>();
    private List<RandomEvents> _questsList = new List<RandomEvents>();

    private List<char> _inputList = new List<char>();

    private PlayState _playState = PlayState.TransitionState;
    private GameState _gameState = GameState.Play;

    [Header("Tweak Values")]
    [SerializeField] private float _habitationMax = 10.0f;
    [SerializeField] private float _foodMax = 10.0f;
    [SerializeField] private float _joyMax = 10.0f;
    [SerializeField] private float _joyLoseRatio = 0.0f;
    [SerializeField] private float _foodGainRatio = 0.0f;
    [SerializeField] private float _HabitationGainRatio = 0.0f;
    [SerializeField] private float _advancementFactor = 0.2f;

    [Header(("UI Elements /!/ GD avoid touching it"))]
    [SerializeField] private Slider _sliderFood;
    [SerializeField] private Slider _sliderHabitation;
    [SerializeField] private Slider _sliderJoy;


    private float _advancementValue = 1.0f;
    private float _innerTimer;
    private float _blowTimer;
    private bool _isFoodDepleting = false;
    private bool _isHabitationDepleting = false;
    private bool _isBlowing;

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

        for (int i = 0; i < EventPool.Count; ++i)
        { 
            switch (EventPool[i].IncantationType)
            {
                case IncantationType.Sun:
                {
                    EventPool[i].EventMelody = new Melody(IncantationType.Sun);
                    break;
                }
                case IncantationType.Health:
                {
                    EventPool[i].EventMelody = new Melody(IncantationType.Health);
                    break;
                }
                case IncantationType.Love:
                {
                    EventPool[i].EventMelody = new Melody(IncantationType.Love);
                    break;
                }
                case IncantationType.Rain:
                {
                    EventPool[i].EventMelody = new Melody(IncantationType.Rain);
                    break;
                }
                default:
                {
                    Debug.Log("Forgot to set Incantation type of the Event : " + EventPool[i].EventName);
                    break;
                }
            }
        }

        for (int i = 0; i < QuestPool.Count; ++i)
        {
            QuestPool[i].EventMelody = new MelodyQuest(IncantationType.None);
        }

        PickQuest();
    }

    private void Update()
    {

        if (Input.GetKeyDown("e"))
        {
            if (_gameState == GameState.Play)
            {
                _inputList.Clear();
                _gameState = GameState.Event;
                Debug.Log("Switched GameState to Event");

            }
            else if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('e');
                Debug.Log("e");
            }
            
        }
        
        if (Input.GetKeyDown("r"))
        {
            if (_gameState == GameState.Play)
            {
                _inputList.Clear();
                _gameState = GameState.Quest;
                Debug.Log("Switched GameState to Quest");
            } else if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('r');
                Debug.Log("r");
            }
        }
        
        if (Input.GetKeyDown("t"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('t');
                Debug.Log("t");
            }
        }

        if (Input.GetKeyDown("y"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('y');
                Debug.Log("y");
            }
        }

        if (Input.GetKeyDown("u"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('u');
                Debug.Log("u");
            }
        }

        if (_blowTimer < 0)
        {
            _isBlowing = false;
        }

        if (Input.GetKey("space"))
        {
            _isBlowing = true;
            _blowTimer = 2.0f;
        }

        if (Input.GetKeyDown("p"))
        {
            if (_gameState != GameState.Pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        if (_questsList.Count == 0)
        {
            PickQuest();
        }

        switch (_gameState)
        {
            case GameState.Event:
            {
                if (_inputList.Count == 3)
                {
                    for (int i = 0; i < _eventsList.Count; ++i)
                    {
                        if (_eventsList[i].EventMelody.ValidateInput(_inputList))
                        {
                            Debug.Log("Melody " + _eventsList[i].EventName + " validated");
                            _eventsList.RemoveAt(i);
                        }
                    }
                    _inputList.Clear();
                    _gameState = GameState.Play;
                    Debug.Log("Switched GameState to Play");
                }
                break;
            }
            case GameState.Quest:
            {
                if (_inputList.Count == 5)
                {
                    if (_questsList[0].EventMelody.ValidateInput(_inputList))
                    {
                        Debug.Log("Melody " + _questsList[0].EventName + " validated");
                        _questsList.RemoveAt(0);
                    }
                    _inputList.Clear();
                    _gameState = GameState.Play;
                    Debug.Log("Switched GameState to Play");
                }
                break;
            }
        }

        if (_innerTimer < 0 && _gameState != GameState.Pause)
        {
            ++_playState;
            if (_playState > (PlayState)2)
                _playState = PlayState.TransitionState;
            UpdatePlayState();
        }
        
        _innerTimer -= Time.deltaTime;
        _blowTimer -= Time.deltaTime;

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

    private void UpdatePlayState()
    {
        switch (_playState)
        {
            case PlayState.TransitionState:
            {
                //Debug.Log("Entering Transition state");
                if((int)_advancementValue < 2)
                    _advancementValue += _advancementFactor;
                _innerTimer = 5.0f;
                break;
            }
            case PlayState.EventState:
            {
                for (int i = 0; i < (int)_advancementValue; ++i)
                {
                    int rand = Random.Range(0, EventPool.Count);
                    _eventsList.Add(EventPool[rand]);
                    Debug.Log("Between " + EventPool.Count + " events, i chose the number " + rand + " " + EventPool[rand].EventName + " the event melody is ");
                    EventPool[rand].EventMelody.ShowMelody();
                }
                break;
            }
            case PlayState.AnswerState:
            {
                //Debug.Log("Entering Answer state");
                _innerTimer = 15.0f;
                break;
            }
            default:
            {
                Debug.Log("Default Resolution of the game state");

                break;
            }
        }
    }

    public void SetJoyMeter(float value)
    {
        JoyMeter += value;
    }

    public void PickQuest()
    {
        int rand = Random.Range(0, QuestPool.Count);

        _questsList.Add(QuestPool[rand]);

        Debug.Log("Pulled Quest from Pool number : " + rand + ", name " + QuestPool[rand].EventName + " melody is : ");
        QuestPool[rand].EventMelody.ShowMelody();
    }

    public void DeleteEventFromList(Melody melodyToDelete)
    {
        for (int i = 0; i < _eventsList.Count; ++i)
        {
            if (_eventsList[i].EventMelody == melodyToDelete)
            {
                _eventsList.RemoveAt(i);
                return;
            }
        }

        for (int i = 0; i < _questsList.Count; ++i)
        {
            if (_questsList[i].EventMelody == melodyToDelete)
            {
                _questsList.RemoveAt(i);
                return;
            }
        }
    }

    public void RegainJoy(float value)
    {
        Mathf.Clamp(value, 0, _joyMax);
    }
}
