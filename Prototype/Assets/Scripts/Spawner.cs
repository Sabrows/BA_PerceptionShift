using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    List<GameObject> characters = new List<GameObject>();

    [SerializeField]
    List<GameObject> alreadySpawnedCharacters;

    [SerializeField]
    List<GameObject> spawnpoints;

    // Start is called before the first frame update
    void Start()
    {
        alreadySpawnedCharacters = new List<GameObject>(characters.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spawn(int roundCounter)
    {
        if (roundCounter > 0)
        {
            //FIXME: hacky, it should be needed to iterate through the whole array every time
            foreach (GameObject c in alreadySpawnedCharacters)
            {
                //c.SetActive(false);
            }
        }

        for (int i = 0; i < spawnpoints.Count; i++)
        {
            //TODO: add random character selection
            GameObject character = characters[i];
            var pos = spawnpoints[i].transform.position;
            var rot = spawnpoints[i].transform.rotation;

            Instantiate(character, pos, rot);
            alreadySpawnedCharacters.Add(character);

            if (characters.Count - 1 != 0)
            {
                characters.Remove(character);
            }
            else
            {
                characters = new List<GameObject>(alreadySpawnedCharacters);
            }
        }
    }
}
