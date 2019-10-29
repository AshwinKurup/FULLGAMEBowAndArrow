using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement2 : MonoBehaviour
{
    public Rigidbody rb;
    public Camera ortho_camera; // ScreenToWorldPoint function, required for touch, cannot work with perspective camera, so must use orthographic camera
    // please make sure the orthographic camera is always in the same place as the main camera
    public float dist_to_angle_multiplier; // the angle the bow rotates is proportionally related by this multipler to changes in the finger movement
    public float force_multiplier; // the force on the arrow is proportionally related to distance of the finger from the arrow by this multiplier
    public float rotate_cos_tip_is_heavy_rate; // rate at which the arrow rotates through the air realistically about it's own x axis cos the tip is heavier than body
    public float wind_speed; 
    public float spin; // angular speed of the arrow spinning about it's personal z axis mid air 
    public Vector3 first_touch_pos = new Vector3(0, 0, 0);
    /// don't adjust this from (0,0,0) it's only public so it can be accessed by BowMovement2 HOPE THIS MEANS THAT BOWMOVEMENT2 ALSO RESETS IT PROPERLY
    /// first_touch_pos is the position of the first touch placed on the screen. 
    /// length of the bowstring pulled will be taken in reference to the distance of the current touch position (once moved) to the first_touch_pos
    /// rather than to the center of the arrow or bow   
    public float prev_angle_x = 0f; // used in calculating change of angle of bow // PLEASE LEAVE THESE AS ZERO BOTH HERE AND IN THE EDITOR
    public float prev_angle_y = 0f; 
    // THE ABOVE TWO VARIABLES ARE MADE PUBLIC SO THE TOUCHROTATE FUNCTION CAN BE USED BY THE SCRIPT BOWMOVEMENT2 
    public float prev_angle_z = 0f; // this is used to make sure the bow doesn't rotate slightly around the z axis
    // which it will if the z angle isn't reset to zero in the touchRotate function because its center hasn't been set properly (to see proof, try rotating the bow in the editor)
    // ... which inherits from this script
    public Transform stringbone; // used so that the arrow will move with the string before it's released
    public float arrow_pullback_multiplier;
    public GameObject OriginalArrow; 
    private float length_pulled; // length of the string pulled (current distance from touch to arrow)
    private bool mid_air_rotation_switch = false;
    private float prev_length_pulled;
    public bool arrow_not_released_bool = true; 
    


    void Start()
    {
        rb.useGravity = false;
        
    }


    /// TODO <summary>
    /// THE LATER ISSUE TO SETTLE HERE IS THAT THE ARROW SHOULD BE PULLED BACK ON IT'S Z AXIS. AND WHEN THE BOW MOVES IT SHOULD MOVE WITH THE BOW
    /// MEANING THAT THE PIVOT POINT NOW USED IS THE ARROW CENTER
    /// THE PIVOT POINT THAT SHOULD BE USED IS THE HANDLE OF THE BOW, THE PIVOT POINT OF THE BOW 
    /// </summary>


    void Update() // apparently this happens after the first few frames start??? 
    {
        arrowStringTrack(); // used outside case TouchPhase.Moved since putting it inside the case makes it track slowly 
                            // this uses arrow_tracker_switch to stop tracking the arrow to the string when the touchphase has ended
       
        if (Input.touchCount > 0) // this just means that a single touch has landed
        {

            //rb.AddForce(0, 0, touch_force, ForceMode.VelocityChange);
            Touch touch = Input.GetTouch(0); // this is the first touch out of the all the touches (from different fingers) that have been put on the screen without, assume that it works like a stack when one finger is removed??? 
            switch (touch.phase)
            {
                case TouchPhase.Moved:
                   

                    rb.useGravity = false; // THIS BRINGS BACK THE SAME ARROW TO THE BOW and not make it be affected by gravity until it's released
                                            // . THIS IS ONLY MEANT TO BE HERE FOR TESTING PHASE. 
                                            // PLS REMOVE THIS AND WRITE CODE TO SPAWN ANOTHER ARROW // kk a bit dumb cos it's not affected by gravity but it will still keep rotating
                                            // cos the rotating through the air like tip is heavy script is still active on it lol 
                                        

                    assignFirstTouchPos(touch); // assigns the first touch position that is compared with the touch position once the finger moves to get the length_pulled 
                    // ... and the object rotation
                    //touchRotate(touch); // rotates the arrow according to finger movement     
                    length_pulled = pythogaros(first_touch_pos, ortho_camera.ScreenToWorldPoint(touch.position));
                    //Vector3 dist_bow_center_to_fletching = new Vector3(0, 0, -4); 
                    //transform.position = stringbone.position + transform.TransformDirection(dist_bow_center_to_fletching); // moving the arrow to stringbone.position causes the arrow center to be moved there, therefore it needs to be pushed back by half the length of the arrow 
                    break;

                case TouchPhase.Ended:
                    
                    rb.useGravity = true;
                    rb.AddRelativeForce(0, 0, -(length_pulled * force_multiplier)); // length_pulled * 10 will be the force on the arrow 
                    
                    // the -(length_pulled * force_multiplier) is made to be negative ever since the cameras were reorientated 180 degrees to compensate for the bow flipping 180 whenever the animation started
                    // therefore at first the arrow was projected positively across it's z, now since the cameras face the other direction the arrow needs to fly in the other direction also 
                    mid_air_rotation_switch = true; // arrow can now start to rotate like it's tip is heavier through the air, and spin about it's z axis
                    // first_touch_pos is not reset to (0,0,0) here because it's already done in the BowMovement2 module 
                    break;
            }

        }

        if (mid_air_rotation_switch) // this is only called when the arrow is released
        {
            transform.Rotate(-rotate_cos_tip_is_heavy_rate, 0, spin, Space.Self); // cos a real arrow kinda rotates to be angled downwards (around it's personal x axis), cos the tip is heavier
            // DOUBT it would be necessary (not in air long enough), but add something that stops the arrow from becoming completely vertical
            // the rotate cos tip is heavy rate is made to be negative ever since the arrow needed to be projected negatively along the z axis instead 
            rb.AddForce(wind_speed, 0, 0, ForceMode.VelocityChange);
        }


    }

    public void assignFirstTouchPos(Touch touch) // ensures that only the first touch on the screen will be assigned to first_touch_pos
    {
        if (first_touch_pos.z == 0) 
            /// if the z of first_touch_pos isn't zero it means that first_touch_pos has already been assigned the first touch.position
            /// and since the arrow is always above the ground when interacted with, z should not be zero once the first touch.position has been assigned
            /// KK genius the problem with this is that it does not reset after the first touch. so all future touch movements for each pull of the bowstring
            /// will take reference from that first touch
        {
            first_touch_pos = ortho_camera.ScreenToWorldPoint(touch.position);
        }
    }


    public void touchRotate(Touch touch, GameObject Weapon) // rotates the bow according to finger movement
    {
        Vector3 release_pos = ortho_camera.ScreenToWorldPoint(touch.position);
        float distance_x_moved = -(first_touch_pos.x - release_pos.x); // the distance moved in the x axis by finger relative to the first touch pos of the first touch
        float distance_y_moved = -(first_touch_pos.y - release_pos.y); // there's a negative sign in front of both the distance_x_moved and distance_y_moved
        // please do not change it back to positive.
        // it needs to be negative ever since the cameras were reorientated 180 degrees to compensate for the bow flipping 180 whenever the animation started

        // the bow will rotate up and down around the x axis
        // the bow will rotate left and right around the y axis
        // therefore the bow's x angle component will be proportional to the distance_y_moved by the finger
        float current_angle_x = -distance_y_moved * dist_to_angle_multiplier; // if the finger pulls string up I want the bow to be angled down
        float current_angle_y = distance_x_moved * dist_to_angle_multiplier; //if the finger pulls string right I want the bow to be angled left
        
        float change_angle_x = current_angle_x - prev_angle_x;
        float change_angle_y = current_angle_y - prev_angle_y;
        float change_angle_z = 0 - prev_angle_z; // change_angle_z is the difference between 0 and the current value of z angle
        // this is used to make sure that the z angle is as close to zero as possible, cos the z angle always changes if this 
        // is not there cos the bow rotation pivot isn't set properly
        // to see evidence of that, try rotating about the x axis manually in the editor and see that the z angle will change 
        Weapon.transform.Rotate(change_angle_x, change_angle_y, change_angle_z, Space.World); 
        prev_angle_x = transform.rotation.eulerAngles.x;
        prev_angle_y = transform.rotation.eulerAngles.y;
        prev_angle_z = transform.rotation.eulerAngles.z;

    }

    public float pythogaros(Vector3 arrow_pos, Vector3 release_pos)
    {
        float x = release_pos.x - arrow_pos.x;
        float y = release_pos.y - arrow_pos.y;
        return Mathf.Sqrt(x * x + y * y);
    }

    private void arrowStringTrack() // makes sure the fletching of the arrow tracks to the stringbone when the string hasn't been released yet
    {
        if (arrow_not_released_bool) // this is turned off when the touchphase has ended
        {
            Vector3 dist_bow_center_to_fletching = new Vector3(0, 0, -4);
            transform.position = stringbone.position + transform.TransformDirection(dist_bow_center_to_fletching); // moving the arrow to stringbone.position causes the arrow center to be moved there, therefore it needs to be pushed back by half the length of the arrow 
        }
        else
        {
            //print("LOOSE"); // arrow is allowed to fly off the string 
        }
    }

    

}