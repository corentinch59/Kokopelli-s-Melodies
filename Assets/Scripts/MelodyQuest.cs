using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MelodyQuest : Melody
{
    private List<char> Charlist = new List<char>();
    private void Start()
    {
        MelodyNotes.Clear();

        Charlist.Add('a');
        Charlist.Add('b');
        Charlist.Add('c');
        Charlist.Add('d');
        

        for (int i = 0; i < 5; ++i)
        {
            int newRand = Random.Range(0, Charlist.Count);
            MelodyNotes.Add(Charlist[newRand]);
        }
    }
}