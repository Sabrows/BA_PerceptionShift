using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System;

public class Raycaster : MonoBehaviour
{
    [Header("Class References")]
    [SerializeField] Controller controller;

    [Header("Development Settings")]
    [SerializeField] float maxHitDuration = 3f;
    [SerializeField] bool lineRendererIsActive = false;

    [HideInInspector] public bool raycastEnabled = true;

    private LineRenderer lineRenderer;
    private Transform headsetTransform;
    private Vector3 fwd;
    private int layerMask;
    private float hitDuration = 0;

    private int[,] positiveHitCounterPerApproach;
    private int[,] negativeHitCounterPerApproach;
    private float[,] positiveHitTimerPerApproach;
    private float[,] negativeHitTimerPerApproach;
    private List<string> choicesLog = new List<string>();
    private int logLineIndex = 0;

    void Awake()
    {
        positiveHitCounterPerApproach = new int[controller.currentProcedure.Length, 1];
        negativeHitCounterPerApproach = new int[controller.currentProcedure.Length, 1];

        positiveHitTimerPerApproach = new float[controller.currentProcedure.Length, 1];
        negativeHitTimerPerApproach = new float[controller.currentProcedure.Length, 1];
    }

    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!lineRendererIsActive)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        headsetTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        if (headsetTransform != null)
        {
            fwd = headsetTransform.forward;
            lineRenderer.SetPosition(0, headsetTransform.position);
            lineRenderer.SetPosition(1, fwd * 100);

            if (raycastEnabled)
            {
                RaycastHit hit;
                if (Physics.Raycast(headsetTransform.position, fwd, out hit, Mathf.Infinity, layerMask))
                {
                    var hitTag = hit.transform.tag;
                    hitDuration += Time.deltaTime;
                    UpdateTimer(hitTag);

                    if (hitDuration >= maxHitDuration)
                    {
                        raycastEnabled = false;
                        if (hitTag == "PositiveHit")
                        {
                            positiveHitCounterPerApproach[controller.currentProcedureIndex, 1]++; //Keep track on hit counter
                            UpdateChoicesLog(hit); //Log tester choice
                        }
                        else if (hitTag == "NegativeHit")
                        {
                            negativeHitCounterPerApproach[controller.currentProcedureIndex, 1]++; //Keep track on hit counter
                            UpdateChoicesLog(hit); //Log tester choice
                        }

                        //Reset hitDuration for next round
                        hitDuration = 0f;
                        controller.TriggerNextRound();
                    }
                }
            }
        }
        else
        {
            StartCoroutine("FindHeadset");
        }
    }

    private IEnumerator FindHeadset()
    {
        headsetTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        yield return new WaitForSeconds(1f);
    }

    private void UpdateTimer(string hitObjectTag)
    {
        var currProcIndex = controller.currentProcedureIndex;
        switch (hitObjectTag)
        {
            case "PositiveHit":
                positiveHitTimerPerApproach[currProcIndex, 1] += Time.deltaTime;
                break;

            case "NegativeHit":
                negativeHitTimerPerApproach[currProcIndex, 1] += Time.deltaTime;
                break;

            default:
                Debug.Log("[Debug Note] Hit Object Tag: " + hitObjectTag + " not found!");
                break;
        }
    }

    private void UpdateChoicesLog(RaycastHit hitObject)
    {
        var currProcIndex = controller.currentProcedureIndex;
        var currApproachName = controller.currentProcedure[currProcIndex];
        var currRoundIndex = controller.currentRoundIndex;

        var logLine = logLineIndex + ". In APPROACH: " + currApproachName + " at ROUND: " + currRoundIndex;
        logLine += " the tester CHOSE: " + hitObject.transform.name + "." + Environment.NewLine;

        choicesLog.Add(logLine);
        logLineIndex++;
    }


    public int GetHitCounter(string wantedCounterName, int approachDataIndex)
    {
        switch (wantedCounterName)
        {
            case "positiveHitCounter":
                return positiveHitCounterPerApproach[approachDataIndex, 1];

            case "negativeHitCounter":
                return negativeHitCounterPerApproach[approachDataIndex, 1]; ;

            default:
                Debug.Log("[Debug Note] Wanted Counter: " + wantedCounterName + " not found!");
                return -1;
        }
    }

    public TimeSpan GetTimer(string wantedTimerName, int approachDataIndex)
    {
        TimeSpan temp = new TimeSpan();
        switch (wantedTimerName)
        {
            case "positiveHitTimer":
                temp = TimeSpan.FromSeconds(positiveHitTimerPerApproach[approachDataIndex, 1]);
                return temp;

            case "negativeHitTimer":
                temp = TimeSpan.FromSeconds(negativeHitTimerPerApproach[approachDataIndex, 1]);
                return temp;

            default:
                Debug.Log("[Debug Note] Wanted Timer: " + wantedTimerName + " not found!");
                return TimeSpan.Zero;
        }
    }

    public List<string> GetChoicesLog()
    {
        return choicesLog;
    }
}
