using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

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
    [SerializeField] private TMP_Text _counterText;
    [SerializeField] private TMP_Text _gameovercounterText;
    [SerializeField] private GameObject _gameOverOverlay;
    public Image Bouton1Image;
    public Image Bouton2Image;
    public Image Bouton3Image;
    public Image Bouton4Image;
    public Image Bouton5Image;
    public GameObject EventRainImage;
    public GameObject EventLoveImage;
    public GameObject EventHealthImage;
    public GameObject EventSunImage;

    private float _advancementValue = 1.0f;
    private float _innerTimer;
    private float _blowTimer;
    private bool _isFoodDepleting = false;
    private bool _isHabitationDepleting = false;
    private bool _isBlowing;
    private int _dayCounter = 1;

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
        _gameOverOverlay.SetActive(false);
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
            Bouton1Image.color = new Color32(0,0,255,255);
        }

        if (Input.GetKeyUp("e"))
        {
            Bouton1Image.color = Color.white;
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
            Bouton2Image.color = new Color32(255,0,255,255);
        }

        if (Input.GetKeyUp("r"))
        {
            Bouton2Image.color = Color.white;
        }

        if (Input.GetKeyDown("t"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('t');
                Debug.Log("t");
            }
            Bouton3Image.color = new Color32(255, 0, 0, 255);
        }

        if (Input.GetKeyUp("t"))
        {
            Bouton3Image.color = Color.white;
        }

        if (Input.GetKeyDown("y"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('y');
                Debug.Log("y");
            }
            Bouton4Image.color = new Color32(255, 255, 0, 255);
        }

        if (Input.GetKeyUp("y"))
        {
            Bouton4Image.color = Color.white;
        }

        if (Input.GetKeyDown("u"))
        {
            if (_gameState == GameState.Event && _isBlowing || _gameState == GameState.Quest && _isBlowing)
            {
                _inputList.Add('u');
                Debug.Log("u");
            }
            Bouton5Image.color = new Color32(0, 255, 0, 255);
        }

        if (Input.GetKeyUp("u"))
        {
            Bouton5Image.color = Color.white;
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
                    Bouton1Image.color = Color.white;
                    Bouton2Image.color = Color.white;
                    Bouton3Image.color = Color.white;
                    Bouton4Image.color = Color.white;
                    Bouton5Image.color = Color.white;
                    _gameState = GameState.Play;
                    Debug.Log("Switched GameState to Play");
                }

                switch (_questsList[0].EventMelody.MelodyNotes[_inputList.Count])
                {
                    case 'e':
                    {
                        Bouton1Image.color = new Color32(255, 0, 255, 255);
                        Bouton2Image.color = Color.white;
                        Bouton3Image.color = Color.white;
                        Bouton4Image.color = Color.white;
                        Bouton5Image.color = Color.white;
                        break;
                    }
                    case 'r':
                    {
                        Bouton1Image.color = Color.white;
                        Bouton2Image.color = new Color32(255, 0, 255, 255);
                        Bouton3Image.color = Color.white;
                        Bouton4Image.color = Color.white;
                        Bouton5Image.color = Color.white;
                        break;
                    }
                    case 't':
                    {
                        Bouton1Image.color = Color.white;
                        Bouton2Image.color = Color.white;
                        Bouton3Image.color = new Color32(255, 0, 255, 255);
                        Bouton4Image.color = Color.white;
                        Bouton5Image.color = Color.white;
                        break;
                    }
                    case 'y':
                    {
                        Bouton1Image.color = Color.white;
                        Bouton2Image.color = Color.white;
                        Bouton3Image.color = Color.white;
                        Bouton4Image.color = new Color32(255, 0, 255, 255);
                        Bouton5Image.color = Color.white;
                        break;
                    }
                    case 'u':
                    {
                        Bouton1Image.color = Color.white;
                        Bouton2Image.color = Color.white;
                        Bouton3Image.color = Color.white;
                        Bouton4Image.color = Color.white;
                        Bouton5Image.color = new Color32(255, 0, 255, 255);
                        break;
                    }
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

        EventRainImage.SetActive(false);
        EventLoveImage.SetActive(false);
        EventHealthImage.SetActive(false);
        EventSunImage.SetActive(false);

        for (int i = 0; i < _eventsList.Count; ++i)
        {
            FoodMeter -= _eventsList[i].FoodValue * Time.deltaTime;
            HabitationMeter -= _eventsList[i].HabitationValue * Time.deltaTime;

            switch (_eventsList[i].IncantationType)
            {
                case IncantationType.Rain:
                {
                    EventRainImage.SetActive(true);
                    break;
                }
                case IncantationType.Love:
                {
                    EventLoveImage.SetActive(true);
                    break;
                }
                case IncantationType.Health:
                {
                    EventHealthImage.SetActive(true);
                    break;
                }
                case IncantationType.Sun:
                {
                    EventSunImage.SetActive(true);
                    break;
                }
            }
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

        if (FoodMeter <= 0 || HabitationMeter <= 0 || JoyMeter <= 0)
        {
            Time.timeScale = 0;
            _gameOverOverlay.SetActive(true);
            _gameovercounterText.SetText(_dayCounter.ToString());
        }
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
                _dayCounter += 1;
                _counterText.SetText(_dayCounter.ToString());
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
                _innerTimer = 20.0f;
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
        if (JoyMeter > _joyMax)
            JoyMeter = _joyMax;
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

    public void RestartGame()
    {
        Application.Quit();
    }
}
