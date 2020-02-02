using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnSheep : MonoBehaviour
{
    public float destroyTimeout;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Sheep")
        {
            Destroy(other.gameObject, destroyTimeout);
        }
    }
}
