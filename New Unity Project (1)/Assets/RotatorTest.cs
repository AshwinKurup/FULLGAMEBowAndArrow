using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTest : MonoBehaviour
{
    public Camera ortho_camera;
    // TODO THIS HAS NOT BEEN ASSIGNED AS A RIGIDBODY YET
    //public Camera ortho_camera; // tbh doubt you need a camera for this 
    // for now rotate around the y and z axes
    // but okay nvm x and y also can I just wanna see rotation first
    void Update()
    {
        if (Input.touchCount > 0) // this just means that a single touch has landed

        {
            //rb.AddForce(0, 0, touch_force, ForceMode.VelocityChange);
            Touch touch = Input.GetTouch(0); // this is the first touch out of the all the touches that have been put on the screen without, assume that it works like a stack when one finger is removed??? 
            switch (touch.phase)
            {
                // TODO the bow doesn't rotate around its own center lol 
                case TouchPhase.Moved:
                    Vector3 release_pos = ortho_camera.ScreenToWorldPoint(touch.position);
                    float x_moved = transform.position.x - release_pos.x; // the distance moved in the x axis by finger relative to bow
                    float y_moved = transform.position.y - release_pos.y;
                    transform.Rotate(x_moved*0.1f, y_moved*0.1f, 0, Space.World);

                   

                    break;
                    // if lazy just use trig then find the angle
                    // or could just use the displacement of the x and y to see
                    // ... how it should rotate
                    

                case TouchPhase.Ended:
                    //rb.useGravity = true;
                    //rb.AddForce(0, 0, length_pulled * 100); // length_pulled * 10 will be the force on the arrow 
                    break;
            }

        }
    }
}



