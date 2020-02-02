using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public float maxOxygen = 100.0f;
    public float currentOxygen;

    public float oxygenFlowPerRefresh = 1.0f;
    public float oxygenAcquiredPerSheep = 10.0f;

    private WaitForSeconds depleteCooldown = new WaitForSeconds(2.0f);

    // Start is called before the first frame update
    void Start()
    {
        currentOxygen = maxOxygen;
        StartCoroutine(DepleteOxygen());
    }

    private IEnumerator DepleteOxygen()
    {
        while (true)
        {
            currentOxygen -= oxygenFlowPerRefresh;

            yield return depleteCooldown;
        }
    }
}
