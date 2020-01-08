using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRTK;
using System;
using UnityEngine.SceneManagement;

public class RaycastController : MonoBehaviour
{
    Transform headsetTransform;
    Vector3 fwd;
    LineRenderer lineRenderer;
    private int layerMask;
    private bool foundHeadset = false;

    private bool inIntroScene = true;

    [SerializeField]
    SessionDataController sessionDataController;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //src: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            inIntroScene = true;
        }
        // TODO: add check for approach scenes
        else inIntroScene = false;

    }

    IEnumerator FindHeadset()
    {
        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Update()
    {
        //src: https://answers.unity.com/questions/1104868/start-and-stop-time-help.html
        // TODO: count time for random approach
        if (inIntroScene)
        {
            sessionDataController.sessionData.totalPlaytime_Intro += Time.deltaTime;
        }

        headsetTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        if (headsetTransform != null)
        {
            fwd = headsetTransform.forward;
            lineRenderer.SetPosition(0, headsetTransform.position);
        }
        else
        {
            StartCoroutine("FindHeadset");
            //lineRenderer.SetPosition(0, headsetTransform.position);
        }

        RaycastHit hit;
        if (Physics.Raycast(headsetTransform.position, fwd, out hit, Mathf.Infinity, layerMask))
        {
            string hitTag = hit.transform.tag;

            switch (hitTag)
            {
                case "PositiveHit":
                    sessionDataController.sessionData.positiveHitCounter_Intro += 1;
                    break;

                case "NegativeHit":
                    sessionDataController.sessionData.negativeHitCounter_Intro += 1;
                    break;

                default:
                    break;
            }

            //lineRenderer.SetPosition(1, hit.point);
            
            // Debug.Log("Did hit " + hit.transform.name);

            /*var minutes = Mathf.Floor(time / 60);
            var seconds = time % 60; //Use the euclidean division for the seconds.
            var fraction = (time * 100) % 100;*/

            //update the label value
            //text.text = string.Format("{0:00} : {1:00} : {2:00}", minutes, seconds, fraction);
        }
        else
        {
            //lineRenderer.SetPosition(1, fwd * 500);
        }
    }
}