using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMe : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerStatsManager playerStatsScript = collision.gameObject.GetComponent<PlayerStatsManager>();
            playerStatsScript.currentOxygen += playerStatsScript.oxygenAcquiredPerSheep;
            if(playerStatsScript.currentOxygen > playerStatsScript.maxOxygen)
            {
                playerStatsScript.currentOxygen = playerStatsScript.maxOxygen;
            }
            Destroy(gameObject);
        }
    }
}
