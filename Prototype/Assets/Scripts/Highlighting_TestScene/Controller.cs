using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
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

    private string currentProcedureName;
    private TestData testData;
    private bool isInRound = false;
    private bool isInApproach = false;
    private float[,] timerPerApproach;
    private float[,] roundTimerPerApproach;
    private Dictionary<int, string>[] roundTimersDictionaries;

    void Awake()
    {
        timerPerApproach = new float[currentProcedure.Length, 1];
        roundTimerPerApproach = new float[currentProcedure.Length, amountOfRounds];
        roundTimersDictionaries = new Dictionary<int, string>[currentProcedure.Length];
        testData = InitializeTestData(testData);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (procedures == Procedures.A)
        {
            currentProcedure = testProcedure_A;
            currentProcedureName = "testProcedure_A";
        }
        else
        {
            currentProcedure = testProcedure_B;
            currentProcedureName = "testProcedure_B";
        }

        TriggerNextRound();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateApproachTimer();
        UpdateRoundTimer();
    }

    private TestData InitializeTestData(TestData testDataToInitialize)
    {
        testDataToInitialize = new TestData
        {
            testID = (System.DateTime.Now).ToString(), //TEST ID
            approachData = new ApproachData[currentProcedure.Length], //APPROACH DATA []
        };

        return testDataToInitialize;
    }

    private void TriggerSpawn()
    {
        isInRound = true; //Start roundTimer
        isInApproach = true; //Start approachTimer
        spawner.Spawn(currentRoundIndex, currentProcedure[currentProcedureIndex]);
        raycaster.raycastEnabled = true; //re-enable raycast
    }

    private void EndTest()
    {
        testDataSaver.SaveTestDataToFile(CollectTestData(testData)); //Save data finally
        UnityEditor.EditorApplication.isPlaying = false; //End Application
    }

    private TestData CollectTestData(TestData testDataToFill)
    {
        /* General Data */
        testDataToFill.selectedTestProcedureName = currentProcedureName; //SELECTED TEST PROCEDURE
        testDataToFill.usedRandomSeed = spawner.GetUsedRandomSeed(); //USED RANDOM SEED
        testDataToFill.positiveCharacterOrderNameList = spawner.GetCharacterList("positiveCharacterOrderNameList"); //POSITIVE CHARACTER ORDER NAME LIST
        testDataToFill.negativeCharacterOrderNameList = spawner.GetCharacterList("negativeCharacterOrderNameList"); //NEGATIVE CHARACTER ORDER NAME LIST

        /* Per Approach Data */
        for (int i = 0; i < testDataToFill.approachData.Length; i++)
        {
            //* Counter *//
            int temPosCounter = raycaster.GetHitCounter("positiveHitCounter", i);
            int tempNegCounter = raycaster.GetHitCounter("negativeHitCounter", i);
            if (temPosCounter != -1)
            {
                testDataToFill.approachData[i].positiveHitCounter = temPosCounter; //APPROACH DATA [] POSITIVE HIT COUNTER
            }

            if (tempNegCounter != -1)
            {
                testDataToFill.approachData[i].negativeHitCounter = tempNegCounter; //APPROACH DATA [] NEGATIVE HIT COUNTER
            }

            //* Timer *//
            TimeSpan tempApprTimer = TimeSpan.FromSeconds(timerPerApproach[i, 1]);
            testDataToFill.approachData[i].approachTimer = FormatTimeSpan(tempApprTimer); //APPROACH DATA [] APPROACH TIMER

            TimeSpan temPosTimer = raycaster.GetTimer("positiveHitTimer", i);
            TimeSpan tempNegTimer = raycaster.GetTimer("negativeHitTimer", i);
            if (temPosTimer != TimeSpan.Zero)
            {
                testDataToFill.approachData[i].positiveHitTimer = FormatTimeSpan(temPosTimer); //APPROACH DATA [] POSITIVE HIT TIMER
            }

            if (tempNegTimer != TimeSpan.Zero)
            {
                testDataToFill.approachData[i].negativeHitTimer = FormatTimeSpan(tempNegTimer); //APPROACH DATA [] NEGATIVE HIT TIMER
            }

            
            testDataToFill.approachData[i].roundTimers = ; //TODO: APPROACH DATA [] ROUND TIMERS
        }

        /* Log Data */
        testDataToFill.characterSpawnsLog = spawner.GetSpawnsLog(); //CHARACTER SPAWNS LOG
        testDataToFill.testerChoicesLog = raycaster.GetChoicesLog(); //TESTER CHOICES LOG

        return testDataToFill;
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        string formattedTimeSpanString = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
        //FIXME: If result doesn't look good, try this (from https://gamedev.stackexchange.com/questions/80968/cant-consistently-add-to-timespan-for-unity-game)
        //string formattedTimeSpanString = String.Format("{0}:{1}:{2}",timeSpan.Minutes,timeSpan.Seconds, timeSpan.Milliseconds);
        return formattedTimeSpanString;
    }

    private void UpdateApproachTimer()
    {
        if (isInApproach)
        {
            timerPerApproach[currentProcedureIndex, 1] += Time.deltaTime;
        }
    }

    private void UpdateRoundTimer()
    {
        if (isInRound)
        {
            roundTimerPerApproach[currentProcedureIndex, currentRoundIndex] += Time.deltaTime;
        }
    }

    private void SaveRoundTimer()
    {
        Dictionary<int, string> roundTimers = new Dictionary<int, string>();
        var intKey = currentRoundIndex;

        //Get and Format roundTimer value

        for (int i = 0; i < )
        TimeSpan tempValue = TimeSpan.FromSeconds(roundTimerPerApproach[currentProcedureIndex, currentRoundIndex]);
        var stringValue = FormatTimeSpan(tempValue);

        roundTimers.Add(intKey, stringValue); // Save into Dictionary
    }


    public void TriggerNextRound()
    {
        isInRound = false; //Stop roundTimer
        currentRoundIndex++; //Keep track on currentRound
        if (currentRoundIndex >= amountOfRounds)
        {
            SaveRoundTimer(); //Save round times for approach to dictionary
            isInApproach = false; //Next Approach, stop measuring time
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
}
