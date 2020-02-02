using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITracker : MonoBehaviour
{
    public Image healthBar;
    private PlayerStatsManager statsScript;

    // Start is called before the first frame update
    void Start()
    {
        statsScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = statsScript.currentOxygen / statsScript.maxOxygen;
    }
}
