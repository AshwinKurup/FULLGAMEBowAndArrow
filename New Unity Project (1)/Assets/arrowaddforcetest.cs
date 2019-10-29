using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowaddforcetest : MonoBehaviour
{
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(0, 0, 300);
    }
}
