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
    private int totalPositiveHitCounter = 0;
    private int totalNegativeHitCounter = 0;
    private float totalPositiveTimer = 0f;
    private float totalNegativeTimer = 0f;
    private List<string> choicesLog = new List<string>();
    private int logLineIndex = 0;

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
                            totalPositiveHitCounter++;
                            UpdateChoicesLog(hit); //Log tester choice
                        }
                        else if (hitTag == "NegativeHit")
                        {
                            totalNegativeHitCounter++;
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

    IEnumerator FindHeadset()
    {
        headsetTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        yield return new WaitForSeconds(1f);
    }

    public int GetHitCounter(string wantedCounterName)
    {
        switch (wantedCounterName)
        {
            case "totalTestPositiveHitCounter":
                return totalPositiveHitCounter;

            case "totalTestNegativeHitCounter":
                return totalNegativeHitCounter;

            default:
                return -1;
        }
    }

    void UpdateTimer(string hitObjectTag)
    {
        switch (hitObjectTag)
        {
            case "PositiveHit":
                totalPositiveTimer += Time.deltaTime;
                break;

            case "NegativeHit":
                totalNegativeTimer += Time.deltaTime;
                break;

            default:
                break;
        }
    }

    public TimeSpan GetTimer(string wantedTimerName)
    {
        switch (wantedTimerName)
        {
            case "totalTestPositiveTimer":
                TimeSpan posTimeSpan = TimeSpan.FromSeconds(totalPositiveTimer);
                return posTimeSpan;

            case "totalTestNegativeTimer":
                TimeSpan negTimeSpan = TimeSpan.FromSeconds(totalNegativeTimer);
                return negTimeSpan;

            default:
                return TimeSpan.Zero;
        }
    }

    void UpdateChoicesLog(RaycastHit hitObject)
    {
        var currProcIndex = controller.currentProcedureIndex;
        var approachName = controller.currentProcedure[currProcIndex];
        var currRoundIndex = controller.currentRoundIndex;

        var logLine = logLineIndex + ". In APPROACH: " + controller.currentProcedure[controller.currentProcedureIndex] + " at ROUND: " + controller.currentRoundIndex + " the tester CHOSE: " + hitObject.transform.name + ". \n";
        choicesLog.Add(logLine);
        logLineIndex++;
    }

    public List<string> GetChoicesLog()
    {
        return choicesLog;
    }
}
