using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlock : MonoBehaviour
{
    [SerializeField]
    private bool isPlacingBlock;
    [SerializeField]
    private GameObject staticBlockPrefab;
    [SerializeField]
    private GameObject blockHolder;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isPlacingBlock = true;
            var spawnedBlock = Instantiate(staticBlockPrefab, blockHolder.transform);
            spawnedBlock.transform.position = transform.position;
        }
        else
        {
            isPlacingBlock = false;
        }
    }
}
