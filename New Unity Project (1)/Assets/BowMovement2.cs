using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class BowMovement2 : ArrowMovement2
{

    public float bow_pull_multiplier; // this is multiplied with the distance moved by the finger to be the
                                      // ... amount the bow is pulled (the normalised time through the Draw animation)

    public GameObject Bow; // used for the GameObject parameter of the touchrotate function

    private float draw_length; // the amount the bowstring is drawn


    // private int number_pulls = 0;

    private Animator anim;

    public GameObject ArrowPrefab;
    private GameObject[] ArrowList;
    private int arrow_number = 0; // this is used to reference the current arrow being used from inside the GameObject[] ArrowList;

    private Rigidbody ArrowRigidbody; // used in the temporary arrowspawn function
    private GameObject CurrentArrow;
    private bool arrow_not_spawned = true;

    private Quaternion start_bow_angle;
    private Vector3 start_bow_pos; // both of these are used to make sure that the arrow spawns in the same angle and position as the bow
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("is_idle", true);
        ArrowList = new GameObject[80]; // there can be 80 arrows spawned and released
        start_bow_angle = transform.rotation;
        start_bow_pos = transform.position; // takes the position where the bow is 90 degrees vertical from the ground
        
    }



    // kk now I want the bow to rotate

    

    void Update()
    {
        
        if (Input.touchCount > 0)
        {
            arrow_not_released_bool = true;

            Touch touch = Input.GetTouch(0); // this isn't being reset when the next touch goes in. suspect is the assignfirsttouchpos function 
                                             //print(draw_length);



            resetBowTransformRotation();


            // find a way to keep this outside the stupid touchphase moved brackets cos it'd suck at tracking then 
            switchOffReversedDraw(); // critical that this is used outside of TouchPhase.Moved or TouchPhase.Ended cos sometimes the touches aren't recorded properly
            // and if the reversed_draw boolflag is left on the bow will keep jerking between draw, loose twang idle annoyingly 
            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    //print("touchphasemove is being recorded");
                    assignFirstTouchPos(touch); // assigns the first touch position that is compared with the touch position once the finger moves to get the length_pulled 
                                                // ... and the object rotation
                                                // this edits first_touch_pos
                                                // first_touch_pos is from the ArrowMovement2 function 
                                                // ... first_touch_pos is a public variable. However since public variables appear in the editor and unity priotises the editor's setting of the variable
                                                // ... rather than what the public variable was previously set to by it's original function, first_touch_pos will be zeroed out in this function again therefore has to be set again


                    
                    CurrentArrow = spawnArrow();
                    ArrowRigidbody = CurrentArrow.GetComponent<Rigidbody>();
                    arrowStringTrack(CurrentArrow);

                    touchRotate(touch, CurrentArrow);
                    touchRotate(touch, Bow);
                    
                    anim.SetBool("is_idle", false);
                    anim.SetBool("is_draw", true);
                    draw_length = bow_pull_multiplier * pythogaros(first_touch_pos, ortho_camera.ScreenToWorldPoint(touch.position));
                    
                    anim.SetFloat("draw_length", draw_length); // the 0.1f has to be there cos anim_length_pulled can reach 10 and the draw length
                                                               // ... the normalised time to jump to in the animation, has to be less than 1   

                    break;

                case TouchPhase.Ended:
                    // release the bow 

                    //print("TOUCHPHASE.END IS BEING RECORDED");
                    arrow_not_released_bool = false; // the arrow is released and no longer tracks to the stringbone
                    anim.SetBool("is_draw", false);
                    anim.SetBool("is_reversed_draw", true);
                    anim.Play("ReversedDraw", 0, (1 - draw_length)); // the 0 in the middle means this plays from layer 0. the 0.5f represents the normalised time 
                    anim.SetFloat("draw_length", 0f);
                    first_touch_pos = new Vector3(0, 0, 0); // reset first_touch_pos TODO HOPE THIS RESETS FIRST_TOUCH_POS FOR THE ARROW FUNCTION AS WELL 
                                                            // AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0); // this takes place in the 0th layer
                    looseArrow(ArrowRigidbody);
                    
                    arrow_not_spawned = true; // allows another arrow to be spawned using the spawnArrow() function
                    
                    break;
            }
            
        }
        midAirRot(CurrentArrow);
    }
    void switchOffReversedDraw() //THIS FUNCTION IS A GODSEND THIS MAKES SURE THE BOW DOESN'T GET JERKY WHEN TOUCHPHASE.ENDED ISN'T PROPERLY RECORDED CAUSING THE REVERSED_DRAW BOOLFLAG TO ALWAYS BE TRUE 
    {
        if (anim.GetBool("is_draw"))
        {
            anim.SetBool("is_reversed_draw", false);
        }
    }

    private void midAirRot(GameObject CurrentArrow)
    {
        if (!arrow_not_released_bool) // if the arrow has been released 
        {
            CurrentArrow.transform.Rotate(-rotate_cos_tip_is_heavy_rate, 0, spin, Space.Self); // cos a real arrow kinda rotates to be angled downwards (around it's personal x axis), cos the tip is heavier
        }
    }

    private GameObject spawnArrow()
    {
        if (arrow_not_spawned == true)
        {
            prev_angle_x = 0f; 
            prev_angle_y = 0f;
            prev_angle_z = 0f;
            // the above are used to make sure that the next arrow spawned isn't affect by the prev_angle_x to z values (which are edited in the touchrotate function) as this could cause the arrow to spawn at awkward angles since after it spawns it's already being affected by the touchrotate function
            // right now the arroww jumps a little bit when spawning, might be a framerate issue check if it's present on the phone as well, if not then not an issue for me lol
            GameObject Arrow = Instantiate(ArrowPrefab, start_bow_pos, start_bow_angle) as GameObject;
            //GameObject A2rrow = Instantiate(ArrowPrefab) as GameObject;
            ArrowList[arrow_number] = Arrow;
            //arrowStringTrack(ArrowList[arrow_number]); // why tf is this causes 1 million arrows to spawn in a T shape smh
            arrow_not_spawned = false; // this will be reset when the arrow is released 
            arrow_number++; // the script understands that there has been one more arrow spawned. It will put the next arrow spawned into an index number corresponding
            // to this (increased by 1) arrow number in the ArrowList
        }
        return ArrowList[arrow_number - 1]; //return the current arrow spawned (arrow_number -1) is used cos arrow_number was increased by 1 to make a space for the next arrow spawned after this one in the if conditional above

    }

    void looseArrow(Rigidbody arrow_rb)
    {
        arrow_rb.useGravity = true;
        arrow_rb.AddRelativeForce(0, 0, -5000);
       
    }


    private void resetBowTransformRotation()
    {
        if (arrow_not_spawned)
        {
            transform.rotation = start_bow_angle;
            transform.position = start_bow_pos;
        }
    }

    private void arrowStringTrack(GameObject CurrentArrow) // makes sure the fletching of the arrow tracks to the stringbone when the string hasn't been released yet
    {
        if (arrow_not_released_bool) // this is turned off when the touchphase has ended. Arrow_not_released_bool is used in the parent ArrowMovement2.cs
        {

            Vector3 dist_bow_center_to_fletching = new Vector3(0, 0, -4);
            CurrentArrow.transform.position = stringbone.position + transform.TransformDirection(dist_bow_center_to_fletching); // moving the arrow to stringbone.position causes the arrow center to be moved there, therefore it needs to be pushed back by half the length of the arrow 
        }
        else
        {
            print("LOOSE"); // arrow is allowed to fly off the string 
        }
    }












}
