using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IncantationType
{
    None,
    Rain,
    Sun,
    Health,
    Love
}
public class Melody : MonoBehaviour
{
    public List<char> MelodyNotes = new List<char>();
    public IncantationType Incantation;

    private char _note1 = 'e';
    private char _note2 = 'r';
    private char _note3 = 't';
    private char _note4 = 'y';
    private char _note5 = 'u';

    private List<char> _rainIncantation = new List<char>();
    private List<char> _sunIncantation = new List<char>();
    private List<char> _loveIncantation = new List<char>();
    private List<char> _healthIncantation = new List<char>();

    public Melody(IncantationType incant)
    {
        _rainIncantation.Add(_note5);
        _rainIncantation.Add(_note1);
        _rainIncantation.Add(_note4);

        _sunIncantation.Add(_note3);
        _sunIncantation.Add(_note2);
        _sunIncantation.Add(_note5);

        _healthIncantation.Add(_note1);
        _healthIncantation.Add(_note4);
        _healthIncantation.Add(_note2);

        _loveIncantation.Add(_note4);
        _loveIncantation.Add(_note5);
        _loveIncantation.Add(_note2);

        switch (incant)
        {
            case IncantationType.Rain:
            {
                MelodyNotes = _rainIncantation;
                break;
            }
            case IncantationType.Sun:
            {
                MelodyNotes = _sunIncantation;
                break;
            }
            case IncantationType.Health:
            {
                MelodyNotes = _healthIncantation;
                break;
            }
            case IncantationType.Love:
            {
                MelodyNotes = _loveIncantation;
                break;
            }
        }
    }

    public virtual bool ValidateInput(List<char> input)
    {
        if (input.Count != MelodyNotes.Count)
        {
            Debug.Log("Input count doesn't match");
            return false;
        }

        for (int i = 0; i < MelodyNotes.Count; ++i)
        {
            if (input[i] != MelodyNotes[i])
            {
                Debug.Log("An input doesn't match");
                return false;
            }
        }

        Debug.Log("Input Validated");
        return true;
    }

    public void ShowMelody()
    {
        string melody = " ";
        for (int i = 0; i < MelodyNotes.Count; ++i)
        {
            melody += " " + MelodyNotes[i];
        }
        Debug.Log(melody);
    }
}
