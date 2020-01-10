using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    void Start(){
        //Resize PlayArea to startZone
    }

    void ResetPlayAreaBounds(){
        Debug.Log("Teleported Event fired");
        //Resize PlayArea to endZone
    }
}
