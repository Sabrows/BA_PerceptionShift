using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private int seed = 42;

    [SerializeField]
    List<GameObject> spawnpoints;

    [SerializeField]
    List<GameObject> posCharacters = new List<GameObject>();

    [SerializeField]
    List<GameObject> negCharacters = new List<GameObject>();

    [SerializeField]
    List<GameObject> orderPosCharacters = new List<GameObject>();

    [SerializeField]
    List<GameObject> orderNegCharacters = new List<GameObject>();

    [SerializeField]
    List<GameObject> alreadySpawnedCharacters = new List<GameObject>();

    [SerializeField]
    Material borderHighlight;

    [SerializeField][Range(0.001f, 0.01f)]
    float outlineWidth = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(seed);

        GenerateResultList(seed, posCharacters, orderPosCharacters);

        GenerateResultList(seed, negCharacters, orderNegCharacters);
    }

    // Update is called once per frame
    void Update()
    {
        borderHighlight.SetFloat("g_flOutlineWidth", outlineWidth);
    }

    void GenerateResultList(int seed, List<GameObject> sourceList, List<GameObject> destList)
    {

        string charOrder = "";

        for (int i = 0; i < 10; i++)
        {

            var randomCharIndex = Random.Range(0, sourceList.Count);

            charOrder += "Index: " + randomCharIndex.ToString() + " - ";

            GameObject charToList = sourceList[randomCharIndex];

            destList.Add(charToList);

            charOrder += charToList.name + " || ";

        }

        //Debug.Log(charOrder);

    }

    public void Spawn(int roundCounter, Controller.HighlightingApproaches approach)
    {
        string spawnpointOrder = "";

        //Get random spawnpoint index
        var randomSpawnpointIndex = Random.Range(0, spawnpoints.Count);

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

        //Instantiate pos then neg character at spawnpoints
        //Edit ganeObject tag to identify for later disabling
        //Add to alreadyspawnedChar List

        GameObject spawnedPosChar = Instantiate(orderPosCharacters[roundCounter], pos, rot);
        switch (approach)
        {
            case Controller.HighlightingApproaches.None:
                //Do nothing
                break;

            case Controller.HighlightingApproaches.Border:
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
                break;
        }
        alreadySpawnedCharacters.Add(spawnedPosChar);

        GameObject spawnedNegChar = Instantiate(orderNegCharacters[roundCounter], otherPos, otherRot);
        alreadySpawnedCharacters.Add(spawnedNegChar);

        //Debug String
        spawnpointOrder += "Roundcounter: " + roundCounter + ", Index: " + randomSpawnpointIndex + " >> Spawning " + spawnedPosChar.name + " at " + randomSpawnpoint.name + " || ";
        spawnpointOrder += "Spawning " + spawnedNegChar.name + " at " + otherSpawnpoint.name;

        Debug.Log(spawnpointOrder);
    }

    public void RemovePreviousCharacters()
    {
        foreach (GameObject alreadySpawnedCharacter in alreadySpawnedCharacters)
        {
            Destroy(alreadySpawnedCharacter);
        }
        alreadySpawnedCharacters.Clear();
    }
}
