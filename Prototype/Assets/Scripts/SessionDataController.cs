using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SessionDataController : MonoBehaviour
{
    public SessionData sessionData;

    // Start is called before the first frame update
    void Start()
    {
        sessionData = new SessionData()
        {
            playerID = UnityEngine.Random.Range(0f, 10f),
            sessionTimestamp = System.DateTime.Now,
        };
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("SaveSessionData");
    }

    IEnumerator SaveSessionData()
    {
        string json =  JsonUtility.ToJson(sessionData);
        if (File.Exists(Application.dataPath + "/save.txt")){
            File.WriteAllText(Application.dataPath + "/save.txt", json);
        }
        yield return new WaitForSecondsRealtime(5f);
    }
}
