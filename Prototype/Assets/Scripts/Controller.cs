using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Controller : MonoBehaviour
{

    [SerializeField]
    Spawner spawner;

    [SerializeField]
    Raycaster raycaster;

    public enum HighlightingApproaches
    {
        None,
        Border,
        Arrow
    }

    enum Procedures
    {
        A,
        B
    }

    [SerializeField]
    Procedures procedures = Procedures.A;

    HighlightingApproaches[] testProcedure_A = new HighlightingApproaches[] { HighlightingApproaches.None, HighlightingApproaches.Border, HighlightingApproaches.None };

    HighlightingApproaches[] testProcedure_B = new HighlightingApproaches[] { HighlightingApproaches.None, HighlightingApproaches.Arrow, HighlightingApproaches.None };

    HighlightingApproaches[] currentProcedure;

    [SerializeField]
    //Leave default on 0
    int currentProcedureIndex = 1;

    [SerializeField]
    //Leave default on -1
    int currentRoundIndex = -1;

    [SerializeField][Range(1f, 5f)]
    float timeUntilSpawn = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (procedures == Procedures.A)
        {
            currentProcedure = testProcedure_A;
        }
        else
        {
            currentProcedure = testProcedure_B;
        }

        TriggerNextRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerNextRound()
    {
        //keep track on currentRound
        currentRoundIndex++;

        if (currentRoundIndex >= 10)
        {

            if (currentProcedureIndex == currentProcedure.Length - 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
            currentRoundIndex = 0;
            currentProcedureIndex++;
        }

        //Remove old chars
        spawner.RemovePreviousCharacters();

        Invoke("TriggerSpawn", timeUntilSpawn);
    }

    void TriggerSpawn()
    {
        //spawn new NPCs
        spawner.Spawn(currentRoundIndex, currentProcedure[currentProcedureIndex]);

        //re-enable raycast
        raycaster.raycastEnabled = true;
    }
}
