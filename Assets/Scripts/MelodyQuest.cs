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
        Charlist.Add('a');
        Charlist.Add('b');
        Charlist.Add('c');
        Charlist.Add('d');

        int rand = Random.Range(3, 6);

        for (int i = 0; i < rand; ++i)
        {
            int newRand = Random.Range(0, Charlist.Count);
            MelodyNotes.Add(Charlist[rand]);
        }
    }
}