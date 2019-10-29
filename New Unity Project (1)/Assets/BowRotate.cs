using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowRotate : ArrowMovement2
{
    public GameObject Armature; // this is the child under the parent FirstSeptemberBow
    // the Armature child rotates when the Draw animation starts ...
    // todo continue detailed explanation 
    // or, if lazy, rotate the camera and the entire world so that the draw animation can be compensated for  


    void Start()
    {

       //Armature.transform.eulerAngles = new Vector3(10, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount > 0) // this just means that a single touch has landed
        {

            
            //rb.AddForce(0, 0, touch_force, ForceMode.VelocityChange);
            Touch touch = Input.GetTouch(0); // this is the first touch out of the all the touches that have been put on the screen without, assume that it works like a stack when one finger is removed??? 
            switch (touch.phase)
            {

                case TouchPhase.Moved:
                    // TODO write something here that keeps the bow in place
                    
                    break;

                case TouchPhase.Ended:
                    break;
            }

        }
    }
    

}
