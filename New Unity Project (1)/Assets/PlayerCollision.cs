using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision_info)
    {
        if(collision_info.collider.name == "Cube" || collision_info.collider.name == "Ground") // better to switch to tag rather than name though
        {
            print("yeah the arrow contacted something lol ending game now");
            FindObjectOfType<GameOver>().endGame(); // find object of type kinda just means look for the script not look for an object named that in the game


        }
    }
}
