using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Controller : MonoBehaviour
{
    VRTK_HeadsetFade headsetFade;

    int roundCounter = 0;

    [SerializeField]
    Spawner spawner;

    [SerializeField]
    Raycaster raycaster;

    // Start is called before the first frame update
    void Start()
    {
        headsetFade = GetComponent<VRTK_HeadsetFade>();
        headsetFade.Fade(Color.blue, 5f);

        spawner.Spawn(roundCounter);

        InvokeRepeating("PrintRoundcounter", 0, 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PrintRoundcounter()
    {
        Debug.Log("Roundcounter: " + roundCounter);
    }

    public void nextRound()
    {
        if (roundCounter < 10)
        {
            headsetFade.Fade(Color.black, 2f);
            headsetFade.Unfade(2f);

            //reset hitDuration timer
            raycaster.hitDuration = 0f;
            //FIXME: raycaster not hitting when view faded

            //spawn new NPCs
            spawner.Spawn(roundCounter);

            //keep track on round number
            roundCounter++;

            //re-enable raycast
            raycaster.raycastEnabled = true;

        }
        else
        {
            Debug.Log("Test is over, you reached round: " + roundCounter);
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

}
