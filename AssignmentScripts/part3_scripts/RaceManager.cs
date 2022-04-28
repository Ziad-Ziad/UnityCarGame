using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] allCheckpoints;     // array for the checkpoints and providing refrence for checkpint script
    public int totalLaps = 5;               // defining how many laps we have

    // invoking the racemanager instance 
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // automatic counting for checkpoints in the race tracking 
        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].checkpointNum = i;        // the index of each checkpoint is its number and this is an automatic numbering for the checkpoint
                                                        // you can check it for each one once you start the game and go to each checkpoint
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
