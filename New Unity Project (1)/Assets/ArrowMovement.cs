using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public Rigidbody rb;
    public Camera ortho_camera; // ScreenToWorldPoint function, required for touch, cannot work with perspective camera, so must use orthographic camera
    private float length_pulled; // length of the string pulled
    public float dist_to_angle_multiplier;
    // the angle the bow rotates is proportionally related by this multipler to changes in the finger movement
    private float prev_angle_x = 0f; // used in calculating change of angle of bow
    private float prev_angle_y = 0f;
    void Start()
    {
        rb.useGravity = false;

    }



    void Update() // apparently this happens after the first few frames start??? 
    {

        // actions are touch down, touch pull, touch release; the force is determined by the distance of the touch pulled
        // at first, I want the pull to be just any distance in any direction can, print what the pull force is
        // btw the pull can rotate the bow and arrow as well. Relative to the screen, the bowandarrow rotates about the z and y axes. 
        // cos at first the bow&arrow will be kinda angled facing forward already, cos we're supposed to be behind an invisible soldier
        // ... defending the wall 

        //  kk actually the touch should now pull back the arrow, the cube
        // so FOR NOW the pull will be whatever the distance of touch was from the cube when released, that distance x 100 in force

        // kk even before that have to make gravity not work on the cube until the finger is held then released

        // right now the distance of pull can get by pythogaros

        // kk now shoot it off at the opposite direction of the pull, can find angle w arrow centre???

        if (Input.touchCount > 0) // this just means that a single touch has landed

        {
            //rb.AddForce(0, 0, touch_force, ForceMode.VelocityChange);
            Touch touch = Input.GetTouch(0); // this is the first touch out of the all the touches that have been put on the screen without, assume that it works like a stack when one finger is removed??? 
            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    touchRotate(touch);
                    break;

                case TouchPhase.Ended:
                    //rb.useGravity = true;
                    //rb.AddForce(0, 0, length_pulled * 100); // length_pulled * 10 will be the force on the arrow 
                    break;
            }

        }


    }
    void touchRotate(Touch touch) // rotates the bow according to finger movement
    {
        Vector3 release_pos = ortho_camera.ScreenToWorldPoint(touch.position);
        float distance_x_moved = transform.position.x - release_pos.x; // the distance moved in the x axis by finger relative to bow
        float distance_y_moved = transform.position.y - release_pos.y;
        // the bow will rotate up and down around the x axis
        // the bow will rotate left and right around the y axis
        // therefore the bow's x angle component will be proportional to the distance_y_moved by the finger
        float current_angle_x = -distance_y_moved * dist_to_angle_multiplier; // if the finger pulls string up I want the bow to be angled down
        float current_angle_y = distance_x_moved * dist_to_angle_multiplier; //if the finger pulls string right I want the bow to be angled left
        // TODO find out why distance_x_moved doesn't have to be negatived
        float change_angle_x = current_angle_x - prev_angle_x;
        float change_angle_y = current_angle_y - prev_angle_y;
        transform.Rotate(change_angle_x, change_angle_y, 0, Space.World);
        prev_angle_x = transform.rotation.eulerAngles.x;
        prev_angle_y = transform.rotation.eulerAngles.y;
    }

    float pythogaros(Vector3 arrow_pos, Vector3 release_pos)
    {
        float x = release_pos.x - arrow_pos.x;
        float y = release_pos.y - arrow_pos.y;
        return Mathf.Sqrt(x * x + y * y);
    }


}