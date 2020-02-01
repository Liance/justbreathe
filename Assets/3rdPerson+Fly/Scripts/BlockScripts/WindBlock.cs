using UnityEngine;
using System.Collections;


public class WindBlock : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsAbove;

    // Use this for initialization
    void Start()
    {

    }

    private void FixedUpdate()
    {
        //Raycast upwards for a set distance and get all objects within that area


        //If you find a game object with a rigidbody in that raycast, start a coroutine that applies a force to it.

        ////				behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit))
            print("Found an object - distance: " + hit.distance);

    }


    // Update is called once per frame
    void Update()
    {


    }

}
