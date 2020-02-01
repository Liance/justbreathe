﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameEnvironmentManager : MonoBehaviour
{
    public static int OxygenLevel = 100;
    public static int NumberOfDomeHoles = 0;
    public static bool IsGameOver = false;
    [SerializeField]
    private Text oxygenLevelText;
    [SerializeField]
    private Text gameOverText;

    private void Update()
    {
        if(OxygenLevel <= 0)
        {
            SetGameOverText();
        }
        if(NumberOfDomeHoles == 0)
        {
            SetOxygenLevelText();
        }
    }
    public void SetOxygenLevelText()
    {
        oxygenLevelText.text = (OxygenLevel - NumberOfDomeHoles).ToString();
    }

    public void SetGameOverText()
    {
        gameOverText.text = "Game Over No Oxygen!";
    }
}
