using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MelodyQuest : Melody
{
    private List<char> Charlist = new List<char>();

    private float JoyRegained = 3.5f;

    //private void Start()
    //{
    //    MelodyNotes.Clear();

    //    Charlist.Add('a');
    //    Charlist.Add('b');
    //    Charlist.Add('c');
    //    Charlist.Add('d');
        

    //    for (int i = 0; i < 5; ++i)
    //    {
    //        int newRand = Random.Range(0, Charlist.Count);
    //        MelodyNotes.Add(Charlist[newRand]);
    //    }
    //}

    public MelodyQuest(IncantationType incant) : base(incant)
    {
        Incantation = IncantationType.None;
        MelodyNotes.Clear();

        Charlist.Add('e');
        Charlist.Add('r');
        Charlist.Add('t');
        Charlist.Add('y');
        Charlist.Add('u');


        for (int i = 0; i < 5; ++i)
        {
            int newRand = Random.Range(0, Charlist.Count);
            MelodyNotes.Add(Charlist[newRand]);
        }
    }

    public override bool ValidateInput(List<char> input)
    {
        if (base.ValidateInput(input))
        {
            GameController.Instance.SetJoyMeter(JoyRegained);
            return true;
        }

        return false;
    }
}