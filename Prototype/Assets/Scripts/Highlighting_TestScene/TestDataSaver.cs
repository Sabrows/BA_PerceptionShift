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
}
