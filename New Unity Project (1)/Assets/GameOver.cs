using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
   

    // lol csharp is default private
    public void endGame()
    {
        Debug.Log("This is the name of the scene: " + SceneManager.GetActiveScene().name);
        Debug.Log("Game is Over");
        restart();
    }

    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }
}
