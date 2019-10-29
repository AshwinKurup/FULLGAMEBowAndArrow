using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution((int)Screen.width, (int)Screen.height, true);
       // print("this is the resolution of phone screen: " + Screen.currentResolution); // all this print stuff will work when using UNITYREMOTE only 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
