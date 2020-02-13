using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestData
{
    /* General Data */
    public string testID;
    public string selectedTestProcedureName;
    public int usedRandomSeed;
    public List<string> positiveCharacterOrderNameList;
    public List<string> negativeCharacterOrderNameList;

    /* Per Approach Data */
    public ApproachData[] approachData;

    /* Log Data */
    public List<string> characterSpawnsLog;
    public List<string> testerChoicesLog;
}
