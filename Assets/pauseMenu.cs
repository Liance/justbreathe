﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
        //Escape Key
        
    }




    public void Resume()
    {
       pauseMenuUI.SetActive(false);

        Time.timeScale =1f;


        GameIsPaused = false;
    }


    public void Pause()
    {


           pauseMenuUI.SetActive(true);

    Time.timeScale =0f;


        GameIsPaused = true;



    //Camera Freeze
    Camera.main.GetComponent<ThirdPersonOrbitCamBasic>().enabled = false;


  }


    
}
