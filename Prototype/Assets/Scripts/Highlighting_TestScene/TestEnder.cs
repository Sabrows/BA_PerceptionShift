using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StopEditor", 20f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StopEditor()
    {
        UnityEditor.EditorApplication.isPlaying = false; //End Application
    }
}
