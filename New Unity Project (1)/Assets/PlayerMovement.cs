using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   // all the code that you write here only comes into effect when you press play
    public Rigidbody rb;
    // Start is called before the first frame update
    
    void Start()
    {    
        Debug.Log("this is the greatest show!");
        //rb = GetComponent<Rigidbody>(); // GetComponent is the interface, rigidbody is the class. 

    }
    // btw make the graphics of this thing fucking beautiful. Maybe not for the first game but at least going up I wanna make them as epic as a mobie game will allow
    /// <summary>
    ///  or fuck maybe just take it easy it's supposed to be an arcade game anyway
    /// </summary>

    // Update is called once per frame
    void FixedUpdate() // unity likes to use FixedUpdate instead of Update for some reason, but only for physics functions
    {
        if (Input.touchCount > 0) //&& Input.GetTouch(0).phase == TouchPhase.Began) 
        { // TODO now just normalise the screen input to the world
           
            rb.AddForce(0, 0, 200);

        }

    }
}
