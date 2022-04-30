using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] allCheckpoints;     // array for the checkpoints and providing refrence for checkpint script
    public int totalLaps = 5;               // defining how many laps we have

    public CarControl playerCar;            // a reference to the player car
    public List<CarControl> allAICars = new List<CarControl>();     // a list reference to the ai cars
    public int playerPosition;              // the display pos var

    // defining the maximum speed of the ai and player cars, definging the maximum speed and acc varience
    public float aiDefaultSpeed = 30f, playerDefaultSpeed = 30f, SpeedVariance = 3.5f, AccelVariance = 0.5f;


    public bool isStarting;                 // to detect weather the race has started or not
    public float timeBetweenStartCount = 1f;// counter's step
    float startCounter;              
    public int countdownCurrent = 3;       // how many seconds to count




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

        isStarting = true;                  // start the counter once the game started
        startCounter = timeBetweenStartCount;
        UIControl.instance.countDownText.text = countdownCurrent.ToString();    // user interface for the starting counter


    }

    // Update is called once per frame
    void Update()
    {

        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countdownCurrent--;                 // decrement the countdownCurrent var by one
                startCounter = timeBetweenStartCount;
                UIControl.instance.countDownText.text = countdownCurrent.ToString();        // user interface for the starting counter
                if (countdownCurrent == 0)
                {
                    isStarting = false;             // once the counter is 0, the isstarting will be false and the game will start
                    UIControl.instance.countDownText.gameObject.SetActive(false);           // Hide the counter when the counter is 0
                    UIControl.instance.goText.gameObject.SetActive(true);                   // display "GO"  when the counter is 0
                }
            }
        }

        else
        {
            playerPosition = 1;     // the player car defult by defult is the first 

            // iterating through the allaicars list
            foreach (CarControl aiCar in allAICars)
            {
                if (aiCar.currentLap > playerCar.currentLap)
                {
                    playerPosition++;       // if the ai car's lap is more than the player's car lap, the position of the player's car will be incremented
                }

                else if (aiCar.currentLap == playerCar.currentLap)      // if both both ai car and the player's car are in the same lap, run this block
                {

                    if (aiCar.nextCheckpoint > playerCar.nextCheckpoint)
                    {
                        // if the ai car's checkpoint(target) is bigger than the player's checkpoint, the position of the player's car will be incremented
                        playerPosition++;
                    }
                    else if (aiCar.nextCheckpoint == playerCar.nextCheckpoint)
                    {
                        if (Vector3.Distance(aiCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position))
                        {

                            playerPosition++;   // if the ai car's distance from the checkpoint is more than the player's distance from the checkpoint, the position of the player's car will be incremented
                        }
                    }
                }
            }

            UIControl.instance.positionText.text = playerPosition + "/" + (allAICars.Count + 1);        // viewing the player's position on the user interface

            // manage speed variance
            if (playerPosition == 1)
            {
                foreach (CarControl aiCar in allAICars)
                {
                    // if the player's car is no1, the speed of the ai cars will be increased 
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + SpeedVariance, AccelVariance * Time.deltaTime);
                }
                // if the player's car is no1 so its speed will be reduced 

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - SpeedVariance, AccelVariance * Time.deltaTime);
            }
            else
            {
                foreach (CarControl aiCar in allAICars)
                {
                    // if the player's car is NOT no1, the speed of the ai cars will be reduced 
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed - SpeedVariance * ((float)playerPosition / ((float)allAICars.Count + 1)), AccelVariance * Time.deltaTime);
                }
                // if the player's car is NOT no1, the speed of the player's car will be increased 
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + SpeedVariance * ((float)playerPosition / ((float)allAICars.Count + 1)), AccelVariance * Time.deltaTime);
            }
        }

    }
}
