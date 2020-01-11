using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System;

public class Spawner : MonoBehaviour
{
    [Header("Class References")]
    [SerializeField] Controller controller;

    [Header("Seed")]
    [SerializeField] private int seed = 42;

    [Header("Spawnpoints List")]
    [SerializeField] List<GameObject> spawnpoints;

    [Header("Character Lists")]
    [SerializeField] List<GameObject> posCharacters = new List<GameObject>();
    [SerializeField] List<GameObject> negCharacters = new List<GameObject>();
    [SerializeField] List<GameObject> orderPosCharacters = new List<GameObject>();
    [SerializeField] List<GameObject> orderNegCharacters = new List<GameObject>();
    [SerializeField] List<GameObject> alreadySpawnedCharacters = new List<GameObject>();

    [Header("Settings HighlightingApproach Border")]
    [SerializeField] Material borderHighlight;
    [SerializeField] [Range(0.001f, 0.01f)] float outlineWidth = 0.005f;

    [Header("Settings HighlightingApproach Arrow")]
    [SerializeField] GameObject arrow;
    [SerializeField] [Range(1f, 10f)] float arrowHeight;

    private int resultListLength;
    private List<string> spawnsLog = new List<string>();
    private int logLineIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //FIXME: Workaround for odd amountOfRounds since there always needs to be a pair of chars
        if (controller.amountOfRounds % 2 == 0)
        {
            resultListLength = controller.amountOfRounds;
        }
        else resultListLength = controller.amountOfRounds + 1;

        UnityEngine.Random.InitState(seed);
        GenerateOrderList(seed, posCharacters, orderPosCharacters);
        GenerateOrderList(seed, negCharacters, orderNegCharacters);
    }

    // Update is called once per frame
    void Update()
    {
        borderHighlight.SetFloat("g_flOutlineWidth", outlineWidth); //Set material border width according to inspector input
    }

    private void GenerateOrderList(int seed, List<GameObject> sourceList, List<GameObject> destList)
    {
        for (int i = 0; i < resultListLength; i++)
        {
            var randomCharIndex = UnityEngine.Random.Range(0, sourceList.Count);
            GameObject charToList = sourceList[randomCharIndex];
            destList.Add(charToList);
        }
    }

    private void UpdateSpawnsLog(int randSpIndex, GameObject randSp, GameObject otherSp, GameObject spawnedPos, GameObject spawnedNeg)
    {
        var currProcIndex = controller.currentProcedureIndex;
        var currApproachName = controller.currentProcedure[currProcIndex];
        var currRoundIndex = controller.currentRoundIndex;

        var logLine = logLineIndex + ". In APPROACH: " + currApproachName + " at ROUND: " + currRoundIndex;
        logLine += " the RANDOM SPAWNPOINT INDEX was: " + randSpIndex + ". ";
        logLine += spawnedPos.name + " was SPAWNED at " + randSp.name + ", ";
        logLine += spawnedNeg.name + " was SPAWNED at " + otherSp.name + "." + Environment.NewLine;

        spawnsLog.Add(logLine);
        logLineIndex++;
    }


    public void Spawn(int roundCounter, Controller.HighlightingApproaches approach)
    {
        //Get random spawnpoint index
        var randomSpawnpointIndex = UnityEngine.Random.Range(0, spawnpoints.Count);

        //Get spawnpoint with index
        GameObject randomSpawnpoint = spawnpoints[randomSpawnpointIndex];
        var pos = randomSpawnpoint.transform.position;
        var rot = randomSpawnpoint.transform.rotation;

        //Get other spawnpoint
        GameObject otherSpawnpoint = null;
        if (randomSpawnpointIndex == 0)
        {
            otherSpawnpoint = spawnpoints[1];
        }
        else if (randomSpawnpointIndex == 1)
        {
            otherSpawnpoint = spawnpoints[0];
        }
        var otherPos = otherSpawnpoint.transform.position;
        var otherRot = otherSpawnpoint.transform.rotation;

        //Instantiate posCharacter at randomSpawnpoint & add to alreadySpawnedCharacters
        GameObject spawnedPosChar = Instantiate(orderPosCharacters[roundCounter], pos, rot);
        switch (approach)
        {
            case Controller.HighlightingApproaches.None:
                //Do nothing
                break;

            case Controller.HighlightingApproaches.Border:
                //Get meshRenderers and apply second border material
                var meshRenderers = spawnedPosChar.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                {
                    var meshMaterials = meshRenderer.materials;
                    meshMaterials[1] = borderHighlight;
                    meshRenderer.materials = meshMaterials;
                }
                break;

            case Controller.HighlightingApproaches.Arrow:
                //Instantiate Arrow over char Position
                var spawnedPosCharPosition = spawnedPosChar.gameObject.transform.position;
                spawnedPosCharPosition.y += arrowHeight;
                GameObject spawnedArrow = Instantiate(arrow, spawnedPosCharPosition, arrow.gameObject.transform.rotation);
                spawnedArrow.transform.parent = spawnedPosChar.gameObject.transform;
                break;
        }
        alreadySpawnedCharacters.Add(spawnedPosChar);

        //Instantiate negCharacter at otherSpawnpoint & add to alreadySpawnedCharacters
        GameObject spawnedNegChar = Instantiate(orderNegCharacters[roundCounter], otherPos, otherRot);
        alreadySpawnedCharacters.Add(spawnedNegChar);

        //Log spawns
        UpdateSpawnsLog(randomSpawnpointIndex, randomSpawnpoint, otherSpawnpoint, spawnedPosChar, spawnedNegChar);
    }

    public void RemovePreviousCharacters()
    {
        foreach (GameObject alreadySpawnedCharacter in alreadySpawnedCharacters)
        {
            Destroy(alreadySpawnedCharacter);
        }
        alreadySpawnedCharacters.Clear();
    }

    public int GetUsedRandomSeed()
    {
        return seed;
    }

    public List<string> GetCharacterList(string wantedListName)
    {
        List<string> temp = new List<string>();
        switch (wantedListName)
        {
            case "positiveCharacterOrderNameList":
                foreach (GameObject character in orderPosCharacters)
                {
                    temp.Add(character.name);
                }
                return temp;

            case "negativeCharacterOrderNameList":
                foreach (GameObject character in orderNegCharacters)
                {
                    temp.Add(character.name);
                }
                return temp;

            default:
                Debug.Log("[Debug Note] Wanted List: " + wantedListName + " not found!");
                return null;
        }
    }

    public List<string> GetSpawnsLog()
    {
        return spawnsLog;
    }
}
