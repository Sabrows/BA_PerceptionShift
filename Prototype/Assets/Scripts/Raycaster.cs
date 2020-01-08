using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Raycaster : MonoBehaviour
{
    private Transform headsetTransform;
    private Vector3 fwd;
    LineRenderer lineRenderer;

    [SerializeField]
    Controller controller;

    private int layerMask;

    public bool raycastEnabled = true;

    public float hitDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
                    hitDuration += Time.deltaTime;
                    Debug.Log(hit.transform.name + " was hit for " + hitDuration + "sec");
                    //FIXME: check if 10f equals 10 sec
                    if (hitDuration >= 3f)
                    {
                        raycastEnabled = false;
                        controller.nextRound();
                    }
                }
            }
        }
        else
        {
            Debug.Log("headsetTransform is null!");
            StartCoroutine("FindHeadset");
        }
    }

    IEnumerator FindHeadset()
    {
        headsetTransform = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        yield return new WaitForSeconds(1f);
    }
}
