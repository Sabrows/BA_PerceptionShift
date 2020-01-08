using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SessionDataController : MonoBehaviour
{
    public SessionData sessionData;

    // Start is called before the first frame update
    void Start()
    {
        sessionData = new SessionData()
        {
            sessionTimestamp = System.DateTime.Now,
        };

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            sessionData.sessionID = Guid.NewGuid().ToString();
        }
        else if (SceneManager.GetActiveScene().name.Contains("Approach"))
        {
            sessionData.randomApproachID = SceneManager.GetActiveScene().buildIndex.ToString();
        }
        
        // Invoke dummy saving function starting at 2 sec and repeating every 10 sec
        InvokeRepeating("SaveSessionData", 0.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SaveSessionData()
    {
        string json = JsonUtility.ToJson(sessionData);
        File.AppendAllText(Application.dataPath + "/save.txt", json);
    }
}
