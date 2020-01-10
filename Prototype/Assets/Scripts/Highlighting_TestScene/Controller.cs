using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System.Diagnostics;
using System;

public class Controller : MonoBehaviour
{
    [Header("Class References")]
    [SerializeField] Spawner spawner;
    [SerializeField] Raycaster raycaster;
    [SerializeField] TestDataSaver testDataSaver;

    enum Procedures
    {
        A,
        B
    }

    [Header("Procedure Selection")]
    [SerializeField] Procedures procedures = Procedures.A;

    public enum HighlightingApproaches
    {
        None,
        Border,
        Arrow
    }
    HighlightingApproaches[] testProcedure_A = new HighlightingApproaches[] { HighlightingApproaches.None, HighlightingApproaches.Border, HighlightingApproaches.None };
    HighlightingApproaches[] testProcedure_B = new HighlightingApproaches[] { HighlightingApproaches.None, HighlightingApproaches.Arrow, HighlightingApproaches.None };
    public HighlightingApproaches[] currentProcedure; //public for Log generation

    [Header("Development Settings")]
    [SerializeField] [Range(2, 16)] public int amountOfRounds = 10;
    [SerializeField] [Range(0, 3)] public int currentProcedureIndex = 0; //Default = 0, public for Log generation
    [SerializeField] [Range(-1, 10)] public int currentRoundIndex = -1; //Default = -1, public for Log generation
    [SerializeField] [Range(1f, 5f)] float timeUntilSpawn = 1f;

    private string selectedTestProcedureName;
    TestData testData;
    Stopwatch stopwatch;
    TimeSpan totalTestTimeSpan;
    private bool isInRound = false;
    private float roundTimer = 0f;
    private Dictionary<string, float> roundTimers = new Dictionary<string, float>();

    void Awake()
    {
        testData = CreateNewTestDataInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        stopwatch = new Stopwatch();

        if (procedures == Procedures.A)
        {
            currentProcedure = testProcedure_A;
            selectedTestProcedureName = "testProcedure_A";
        }
        else
        {
            currentProcedure = testProcedure_B;
            selectedTestProcedureName = "testProcedure_B";
        }

        TriggerNextRound();
        stopwatch.Start();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRoundTimer();
    }

    public void TriggerNextRound()
    {
        isInRound = false; //Stop roundTimer
        SaveRoundTimer(); //Save roundTimer to dictionary
        currentRoundIndex++; //Keep track on currentRound
        if (currentRoundIndex >= amountOfRounds)
        {
            CollectTestData(testData); //FIXME: comment in for FINAL
            testDataSaver.SaveTestDataToFile(testData); //FIXME: comment in for FINAL

            if (currentProcedureIndex == currentProcedure.Length - 1) //If true, end test
            {
                EndTest();
                return;
            }
            currentRoundIndex = 0;
            currentProcedureIndex++;
        }
        spawner.RemovePreviousCharacters(); //Remove old NPCs
        Invoke("TriggerSpawn", timeUntilSpawn); //Spawn new NPCs
    }

    void TriggerSpawn()
    {
        spawner.Spawn(currentRoundIndex, currentProcedure[currentProcedureIndex]);
        raycaster.raycastEnabled = true; //re-enable raycast
        isInRound = true; //Start roundTimer
    }

    void EndTest()
    {
        if (stopwatch.IsRunning)
        {
            stopwatch.Stop(); //Stop measuring totalTestTimeSpan
            totalTestTimeSpan = stopwatch.Elapsed;
        }

        //FIXME: uncomment for FINAL
        CollectTestData(testData); //Collect test data finally

        //FIXME: uncomment for FINAL
        testDataSaver.SaveTestDataToFile(testData); //Save data finally

        UnityEditor.EditorApplication.isPlaying = false; //End Application
    }

    //creates new TestData struct and initializes the ID with DateTime
    public TestData CreateNewTestDataInstance()
    {
        TestData testData = new TestData
        {
            testID = (System.DateTime.Now).ToString(), //TODO: TEST ID
        };

        return testData;
    }

    void CollectTestData(TestData testData)
    {
        /* General Info */
        testData.selectedTestProcedure = selectedTestProcedureName; //TODO: SELECTED TEST PROCEDURE
        testData.usedRandomSeed = spawner.GetUsedRandomSeed(); //TODO: USED RANDOM SEED
        testData.shuffledPositiveCharacterOrder = spawner.GetCharacterList("shuffledPositiveCharacterOrder"); //TODO: SHUFFLED POSITIVE CHARACTER ORDER
        testData.shuffledNegativeCharacterOrder = spawner.GetCharacterList("shuffledNegativeCharacterOrder"); //TODO: SHUFFLED NEGATIVE CHARACTER ORDER

        /* Total Test Data */
        int temPosCounter = raycaster.GetHitCounter("totalTestPositiveHitCounter");
        int tempNegCounter = raycaster.GetHitCounter("totalTestNegativeHitCounter");
        if (temPosCounter != -1)
        {
            testData.totalTestPositiveHitCounter = temPosCounter; //TODO: TOTAL TEST POSITIVE HIT COUNTER
        }

        if (tempNegCounter != -1)
        {
            testData.totalTestNegativeHitCounter = tempNegCounter; //TODO: TOTAL TEST NEGATIVE HIT COUNTER
        }

        TimeSpan temPosTimer = raycaster.GetTimer("totalTestPositiveTimer");
        TimeSpan tempNegTimer = raycaster.GetTimer("totalTestNegativeTimer");
        if (temPosTimer != TimeSpan.Zero)
        {
            testData.totalTestPositiveTimer = FormatTimeSpan(temPosTimer); //TODO: TOTAL TEST POSITIVE TIMER
        }

        if (tempNegTimer != TimeSpan.Zero)
        {
            testData.totalTestNegativeTimer = FormatTimeSpan(tempNegTimer); //TODO: TOTAL TEST NEGATIVE TIMER
        }

        if (stopwatch.IsRunning)
        {
            stopwatch.Stop(); //Stop measuring totalTestTimeSpan
            totalTestTimeSpan = stopwatch.Elapsed;
        }
        testData.totalTestTimer = FormatTimeSpan(totalTestTimeSpan); //TODO: TOTAL TEST TIMER

        /* Per-Round Test Data */
        testData.roundTimers = roundTimers; //TODO: ROUND TIMERS

        /* Logs */
        testData.characterSpawnsLog = spawner.GetSpawnsLog(); //TODO: CHARACTER SPAWNS LOG
        testData.testerChoicesLog = raycaster.GetChoicesLog(); //TODO: TESTER CHOICES LOG
    }

    string FormatTimeSpan(TimeSpan timeSpan)
    {
        string formattedTimeSpanString = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
        //FIXME: If result doesn't look good, try this (from https://gamedev.stackexchange.com/questions/80968/cant-consistently-add-to-timespan-for-unity-game)
        //string formattedTimeSpanString = String.Format("{0}:{1}:{2}",timeSpan.Minutes,timeSpan.Seconds, timeSpan.Milliseconds);
        return formattedTimeSpanString;
    }

    void CheckRoundTimer()
    {
        if (isInRound)
        {
            roundTimer += Time.deltaTime;
        }
    }

    void SaveRoundTimer()
    {
        var currApproachName = currentProcedure[currentProcedureIndex].ToString();
        var key = " ";
        if (roundTimers.ContainsKey(currApproachName + "_" + currentRoundIndex))
        {
            key = currApproachName + "2_" + currentRoundIndex;
        }
        else
        {
            key = currApproachName + "_" + currentRoundIndex;
        }

        roundTimers.Add(key, roundTimer); // Save into Dictionary
        roundTimer = 0f; //Reset roundTimer for next round
    }
}
