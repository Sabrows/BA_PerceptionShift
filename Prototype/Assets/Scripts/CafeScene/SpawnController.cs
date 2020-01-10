using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{

    [Range(1, 6)]
    public int amountSmile;

    [Range(1, 6)]
    public int amountAngry;

    public List<RuntimeAnimatorController> kiraAnimatorControllers;

    public List<RuntimeAnimatorController> lewisAnimatorControllers;

    public List<GameObject> spawnpoints;

    public List<GameObject> characterPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        int spawnpointsCounter = spawnpoints.Count;
        int smileContollerCounter = 0;
        int angryControllerCounter = 0;

        for (int i = 0; i < spawnpoints.Count; i++)
        {
            // TODO: Refactor code duplication!
            // Get position and rotation of current Spawnpoint
            var position = spawnpoints[i].transform.position;
            var rotation = spawnpoints[i].transform.rotation;

            // Get index of random characterPrefab to know wether Kira or Lewis animator controllers are needed
            var randomPrefabIndex = Random.Range(0, characterPrefabs.Count);

            // Get random characterPrefab from list of prefabs
            GameObject randomCharacterPrefab = characterPrefabs[randomPrefabIndex];

            // Get random index (0 -> smile, 1 -> angry) to decide wether smile or angry animator controller gets applied
            var randomControllerIndex = Random.Range(0, 2);
            // Debug.Log("randomControllerIndex: " + randomControllerIndex);

            // Apply correct animator controller
            // smile && free
            if (randomControllerIndex == 0 && smileContollerCounter != amountSmile)
            {
                if (randomCharacterPrefab.transform.tag == "Kira")
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = kiraAnimatorControllers[randomControllerIndex] as RuntimeAnimatorController;
                    smileContollerCounter++;
                }
                else
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = lewisAnimatorControllers[randomControllerIndex] as RuntimeAnimatorController;
                    smileContollerCounter++;
                }

                randomCharacterPrefab.GetComponentInChildren<MeshCollider>().tag = "PositiveHit";
                characterPrefabs.Remove(randomCharacterPrefab);
                // Debug.Log("SmileCounter: " + smileContollerCounter + ", AngryCounter :" + angryControllerCounter);
            }
            // angry && free
            else if (randomControllerIndex == 1 && angryControllerCounter != amountAngry)
            {
                if (randomCharacterPrefab.transform.tag == "Kira")
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = kiraAnimatorControllers[randomControllerIndex] as RuntimeAnimatorController;
                    angryControllerCounter++;
                }
                else
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = lewisAnimatorControllers[randomControllerIndex] as RuntimeAnimatorController;
                    angryControllerCounter++;
                }

                randomCharacterPrefab.GetComponentInChildren<MeshCollider>().tag = "NegativeHit";
                characterPrefabs.Remove(randomCharacterPrefab);
                // Debug.Log("SmileCounter: " + smileContollerCounter + ", AngryCounter :" + angryControllerCounter);
            }
            // smile && locked
            else if (randomControllerIndex == 0 && smileContollerCounter == amountSmile)
            {
                if (randomCharacterPrefab.transform.tag == "Kira")
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = kiraAnimatorControllers[randomControllerIndex + 1] as RuntimeAnimatorController;
                    angryControllerCounter++;
                }
                else
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = lewisAnimatorControllers[randomControllerIndex + 1] as RuntimeAnimatorController;
                    angryControllerCounter++;
                }

                randomCharacterPrefab.GetComponentInChildren<MeshCollider>().tag = "NegativeHit";
                characterPrefabs.Remove(randomCharacterPrefab);
                // Debug.Log("SMILE && LOCKED - SmileCounter: " + smileContollerCounter + ", AngryCounter :" + angryControllerCounter);
            }
            // angry && locked
            else if (randomControllerIndex == 1 && angryControllerCounter == amountAngry)
            {
                if (randomCharacterPrefab.transform.tag == "Kira")
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = kiraAnimatorControllers[randomControllerIndex - 1] as RuntimeAnimatorController;
                    smileContollerCounter++;
                }
                else
                {
                    randomCharacterPrefab.GetComponent<Animator>().runtimeAnimatorController = lewisAnimatorControllers[randomControllerIndex - 1] as RuntimeAnimatorController;
                    smileContollerCounter++;
                }

                randomCharacterPrefab.GetComponentInChildren<MeshCollider>().tag = "PositiveHit";
                characterPrefabs.Remove(randomCharacterPrefab);
                // Debug.Log("ANGRY && LOCKED - SmileCounter: " + smileContollerCounter + ", AngryCounter :" + angryControllerCounter);
            }

            // Instantiate characterPrefab at current Spawnpoint position with rotation
            Instantiate(randomCharacterPrefab, position, rotation);
            // Debug.Log(randomCharacterPrefab.name);

        }
    }
}
