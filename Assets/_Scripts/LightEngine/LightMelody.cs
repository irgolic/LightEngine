using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Verse
{
    public int Repetitions;
    public bool[] steps;
}

public class LightMelody : MonoBehaviour
{

    public int stepsPerBeat;
    [Header("Verses are variable length, be wary of what stepsPerBeat is set to.")]
    public Verse[] verses;

    int iStep;
    int iRepetitions;
    int iVerse;
    bool over;

    public void Awake()
    {
        iStep = 0;
        iRepetitions = 0;
        iVerse = 0;
    }

    internal bool isStepMelody()
    {
        if (over)
        {
            return false;
        }

        Verse verse = verses[iVerse];

        bool stepBool = verse.steps[iStep];
        if (++iStep >= verse.steps.Length)
        {
            iStep = 0;
            if (++iRepetitions >= verse.Repetitions)
            {
                iRepetitions = 0;
                if (++iVerse >= verses.Length)
                {
                    over = true;
                }
            }
        }

        return stepBool;
    }

}
