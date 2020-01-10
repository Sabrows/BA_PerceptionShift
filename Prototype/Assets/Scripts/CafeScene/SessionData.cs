using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SessionData
{
    public string sessionID;
    public DateTime sessionTimestamp;

    // Intro Scene Data
    public float totalPlaytime_Intro;
    public int positiveHitCounter_Intro;
    public int negativeHitCounter_Intro;
    public float positiveHitTime_Intro;
    public float negativeHitTime_Intro;

    // Random Approach Scene Data
    public string randomApproachID;
    public float totalPlaytime_randomApproach;
    public int positiveHitCounter_randomApproach;
    public int negativeHitCounter_randomApproach;
    public float positiveHitTime_randomApproach;
    public float negativeHitTime_randomApproach;

}
