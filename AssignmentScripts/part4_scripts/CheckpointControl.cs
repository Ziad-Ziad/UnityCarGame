using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointControl : MonoBehaviour
{
    public CarControl carcontrol;      // invoking the carcontrol script to use it in this script
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")          // interacting with the checkpoints using their tag
        {
            
            // print("Hit checkpoint: " + other.GetComponent<Checkpoint>().checkpointNum);     // when there is a collision between a checkpoint and and object, print that checkpoint number
            
            carcontrol.CheckpointHit(other.GetComponent<Checkpoint>().checkpointNum);       // when 
        }
    }
}
