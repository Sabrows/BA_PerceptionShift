using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TestData
{
    /* General Info */
    public string testID;
    public string selectedTestProcedure;
    public int usedRandomSeed;
    public List<GameObject> shuffledPositiveCharacterOrder;
    public List<GameObject> shuffledNegativeCharacterOrder;

    /* Total Test Data */
    public int totalTestPositiveHitCounter;
    public int totalTestNegativeHitCounter;
    public string totalTestPositiveTimer;
    public string totalTestNegativeTimer;
    public string totalTestTimer;

    /* Per-Round Test Data */
    public Dictionary<string, float> roundTimers;

    /* Logs */
    public List<string> characterSpawnsLog;
    public List<string> testerChoicesLog;
}
