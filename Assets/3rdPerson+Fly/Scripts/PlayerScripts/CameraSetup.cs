using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{

    [SerializeField]
    private GameObject activatableCamera;
    // Start is called before the first frame update
    void Start()
    {
        activatableCamera.GetComponent<Camera>().enabled = false;
        activatableCamera.GetComponent<Camera>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
