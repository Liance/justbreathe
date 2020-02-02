using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpaceProgram : MonoBehaviour
{
    public GameObject sheepOfTruth;

    public List<GameObject> spawnedSheep;

    private WaitForSeconds spawnCooldown = new WaitForSeconds(3.0f);

    public float pushforce = 10.0f;

    public float microgravityPush = 2.0f;

    private Quaternion spawnQ = new Quaternion();

    private bool spawnerOn = false;

    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        spawnerOn = true;
        StartCoroutine(SheepSpawner());              
    }

    private IEnumerator SheepSpawner()
    {
        while (spawnerOn)
        {
            GameObject spawn = Instantiate(sheepOfTruth, transform.position, spawnQ, null);
            spawn.transform.LookAt(playerObject.transform);
            spawn.gameObject.GetComponent<Rigidbody>().AddForce(spawn.transform.forward * pushforce, ForceMode.Impulse);
            spawnedSheep.Add(spawn);
            yield return spawnCooldown;
            Debug.Log(Physics.gravity.y);
            Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y + (1.0f/spawnedSheep.Count) * 2.0f, Physics.gravity.z);           
        }
    }
}
