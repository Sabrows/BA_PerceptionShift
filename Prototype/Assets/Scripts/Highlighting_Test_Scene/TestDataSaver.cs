using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TestDataSaver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SaveTestDataToFile(TestData testDataToSave)
    {
        string savePath = Application.dataPath + ("/TestData/testData.txt");
        string jsonFile = JsonUtility.ToJson(testDataToSave);

        File.AppendAllText(savePath, jsonFile + Environment.NewLine);
    }

    public void SaveApproachDataToFile(ApproachData approachDataToSave)
    {
        string savePath = Application.dataPath + ("/TestData/approachData.txt");
        string jsonFile = JsonUtility.ToJson(approachDataToSave);

        File.AppendAllText(savePath, jsonFile + Environment.NewLine);
    }

    public void SaveLogToFile(string testDataID, List<string> logToSave, string logName)
    {
        logToSave.Insert(0, "testID: " + testDataID); //Place testDataID to differentiate Test Sessions
        string savePath = Application.dataPath + ("/TestData/Logs/");
        switch (logName)
        {
            case "characterSpawnsLog":
                savePath += "characterSpawns.txt";
                break;
            case "testerChoicesLog":
                savePath += "testerChoices.txt";
                break;
            default:
                Debug.Log("[Debug Note] Log: " + logName + " not found!");
                break;
        }

        string jsonFile = JsonUtility.ToJson(logToSave);
        File.AppendAllText(savePath, jsonFile + Environment.NewLine);
    }
}
