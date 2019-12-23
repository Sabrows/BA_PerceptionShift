using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SessionData
{
    public float playerID;
    public DateTime sessionTimestamp;
    public Time totalSessionPlaytime;
    public int positiveHitCounter;
    public int negativeHitCounter;
    public Time positiveHitTimer;
    public Time negativeHitTimer;
}
