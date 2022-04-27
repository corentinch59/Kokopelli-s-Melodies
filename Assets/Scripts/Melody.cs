using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melody : MonoBehaviour
{
    public List<char> MelodyNotes = new List<char>();

    public bool ValidateInput(List<char> input)
    {
        for (int i = 0; i < MelodyNotes.Count; ++i)
        {
            if (input[i] != MelodyNotes[i])
            {
                return false;
            }
        }

        return true;
    }
}
