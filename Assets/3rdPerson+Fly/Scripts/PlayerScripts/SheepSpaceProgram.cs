using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpaceProgram : MonoBehaviour
{
    public GameObject sheepOfTruth;

    private WaitForSeconds spawnCooldown = new WaitForSeconds(2.0f);

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SheepSpawner());              
    }

    private IEnumerator SheepSpawner()
    {
        while (true)
        {
            Vector3 randomPos = new Vector3(Random.Range(-35.0f, 35.0f), Random.Range(0.0f, 50.0f), Random.Range(-35.0f, 35.0f));
            Debug.Log("Spawned a sheep");
            Instantiate(sheepOfTruth, randomPos, new Quaternion(), null);
            yield return spawnCooldown;
        }
    }
}
