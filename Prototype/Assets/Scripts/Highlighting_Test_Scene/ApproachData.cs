using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ApproachData
{
    //* Counter *//
    public int positiveHitCounter;
    public int negativeHitCounter;

    //* Timer *//
    public string approachTimer;
    public string positiveHitTimer;
    public string negativeHitTimer;
    public List<string> roundTimers;
}
