using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float maxSpeed = 30;
    public Rigidbody RB;
    float speedInput;
    public float forwardAccel = 10f, reverseAccel = 5f;
    float turnStrength = 180f;
    float turnInput;
    public Transform leftfrontWheel, rightfrontWheel;
    public float maxWheeltuen = 25f;
    public bool grounded;               // a boolean var to check whether the car on the ground or not
    public Transform groundRayPoint, groundRayPoint2; // To access the new,empty game object "Groundcheckpoint" used for detecting the ground
    public LayerMask whatIsGround;      // a var stores the current ground
    public float groundRayLength = 1f;  // if the distance between the car and the ground is bigger than this so it is considered to be in the air
    float dragOnGround;
    public float gravityMode = 10f;
    public AudioSource engineSound;     // attaching the engine sound
    public int nextCheckpoint;
    public int currentLap = 1;
    public float lapTime, bestLapTime;

    public bool isAI;                   // a boolean to determine our ai cars
    Vector3 targetPoint;                // pos of the target
    public int currentTarget;           // to define the current target in order to move to the next one
    // (applying acceleration on the car), (adding turning feature to it), (adding diversity in order the ai cars not to drive the same way
    public float aiAccelerateSpeed = 1, aiTurnSpeed = 0.8f, aiReachPointRange = 5f, aiPointVariance = 3f, aiMaxTurn = 25f;
    float aiSpeedInput, aiSpeedMod;     // (control speed of the cars), (make the cars move in differ speed)


    // Start is called before the first frame update
    void Start()
    {
        RB.transform.parent = null;
        dragOnGround = RB.drag;
        UIControl.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;     // Modifying the value of lap counter indicator

        if(isAI)
        {
            // get access to the racemanager script and retreive the checkpoints to make the ai cars moving to the checkpoints
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;

            // invoking a func
            RandimiseAllTarget();

            // making the speed of the car random so they move differently
            aiSpeedMod = Random.Range(0.8f, 2f);

        }

    }

    private void Update()
    {

        if (!RaceManager.instance.isStarting)
        {

            lapTime += Time.deltaTime;  // increment the time by the previous one

            // display the ui only for the player's car
            if (!isAI)
            {
                var timespan = System.TimeSpan.FromSeconds(lapTime);

                //declarng the format of the best lap time and assigning it 
                UIControl.instance.currentLapText.text = string.Format("{0:00}m{1:00}.{2:000}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

                if (Input.GetKeyDown(KeyCode.R))
                {
                    // move the car to the track when the R button is clicked
                    ResetToTrack();
                }
            }


            engineSound.pitch = 1f + (RB.velocity.magnitude / maxSpeed) * 2f;       // Increasing the pitch when the velociy increases to make it more realistic
                                                                                    // print(engineSound.pitch);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!RaceManager.instance.isStarting)
        {

            grounded = false;
            RaycastHit hit;     // Detecting the distance

            Vector3 normalTarget = Vector3.zero;    // basic orienation of the car

            // Detecting whenever the car touches the ground
            if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                grounded = true;            // when the car hits the ground the grounded var will be true
                normalTarget = hit.normal;
            }


            // prevent the player from controlling the ai cars
            if (!isAI)
            {
                speedInput = 0f;
                if (Input.GetAxis("Vertical") > 0)
                {
                    speedInput = Input.GetAxis("Vertical") * forwardAccel;
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    speedInput = Input.GetAxis("Vertical") * reverseAccel;
                }

                turnInput = Input.GetAxis("Horizontal");


            }
            else
            {
                targetPoint.y = transform.position.y;   // moving the ai cars in the y axis

                // comparing the distance between the ai cars  and the target point, if it is smaller then the condition is true
                if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
                {
                    currentTarget++;        // increment our current target by one

                    // when the ai car complete a lap, reset the checkpoints to 0
                    if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
                    {
                        currentTarget = 0;
                    }

                    //adding diversity to the ai cars
                    targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
                    RandimiseAllTarget();
                }

                Vector3 targetDir = targetPoint - transform.position;       // the distance between the ar and the target
                float angle = Vector3.Angle(targetDir, transform.forward);  // Storing the angle between the car and the checkpoint(target) in the angle var
                Vector3 localPos = transform.InverseTransformPoint(targetPoint);    // Obtaining sign of the angle:
                if (localPos.x < 0f)
                {
                    angle = -angle;     // if the position is less than 0 so the angle is negative
                }

                turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

                if (Mathf.Abs(angle) < aiMaxTurn)
                {
                    // when the car moves on straight line 
                    aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
                }
                else
                {
                    // how fast to accelerate the car on turning
                    aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
                }
                speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;


            }

            // rotate the car only on the ground layer
            if (grounded && speedInput != 0)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (RB.velocity.magnitude / maxSpeed), 0));
            }


            leftfrontWheel.localRotation = Quaternion.Euler(leftfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), leftfrontWheel.localRotation.eulerAngles.z);
            rightfrontWheel.localRotation = Quaternion.Euler(rightfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), rightfrontWheel.localRotation.eulerAngles.z);

            // rotate the car to match the normal
            if (grounded)
            {
                transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation; // Allign the car along the slopped plane 

            }

            // accelerate the car only on the ground layer
            if (grounded)
            {
                RB.AddForce(transform.forward * speedInput * 10000f);
                RB.drag = dragOnGround;
            }
            else
            {
                RB.drag = 0.1f;     // decreasing the drag value when the car is on the air to make it move 
                RB.AddForce(-Vector3.up * gravityMode * 100f); //increasing the gravity to pull the car to the ground faser to make it more realistic
            }
            transform.position = RB.position;

            // print(RB.velocity.magnitude);

            if (RB.velocity.magnitude > maxSpeed)
            {
                RB.velocity = RB.velocity.normalized * maxSpeed; // 0...55 -> (0...1) * MAXSpeed -> 0...30 limited for the speed cars
            }
        }

    }


    // Returning the checkpoint index that was hitted by the car
    public void CheckpointHit(int checkpointNum)
    {
        // print(checkpointNum);
        if (checkpointNum == nextCheckpoint)
        {
            nextCheckpoint++;       // if the current checkpoint is equal to the previous checkpoint so the var nextcheckpoint will be increamented by one

            // reseting the checkpoint var when a complete lap is done
            if (nextCheckpoint == RaceManager.instance.allCheckpoints.Length)   // the instance race manager is static and is invoked in its script and accessed the checkpoints and its length
            {
               //  print("works");
                nextCheckpoint = 0;
                LapCompleted();
            }
        }

        // prevent the ai cars from spining and to hit the center of the checkpoint  and then move to the ne
        if (isAI)
        {
            if (checkpointNum == currentTarget)
            {
                currentTarget++;
                if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
                {
                    currentTarget = 0;
                }
                targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
                RandimiseAllTarget();
            }
        }
    }

    public void LapCompleted()
    {
        currentLap++;


        // if the current lap time is less than the bestlap time sp the current one when we start the game the lap time will be the bestlaptime
        if (lapTime < bestLapTime || bestLapTime == 0) // || - or, && - and
        {
            bestLapTime = lapTime;
        }



        if (currentLap <= RaceManager.instance.totalLaps)   // if the race is not finished play the game normally
        {
            lapTime = 0f;

            // display the ui only for the player's car
            if (!isAI)
            {
                var timespan = System.TimeSpan.FromSeconds(bestLapTime);
                UIControl.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

                UIControl.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;     // Modifying the value of lap counter indicator
            }
        }
        else
        {   // if the race is finsihed, run the block of code
            
            if (!isAI)
            {
                isAI = true;        // make the player's car an ai car so that the playler cant control it anymore
                aiSpeedMod = 1;     // constant speed to the player's car
                targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
                RandimiseAllTarget();   // invoking the radnimise function to make the cars move randomly (in different speed...)
                var timespan = System.TimeSpan.FromSeconds(bestLapTime);
                // displaying the best time on the user interface
                UIControl.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

                RaceManager.instance.FinishRace();      // invoking the finsihace function from the race manager script
            }
        }


    }

    // Making the AI cars move differently (In differ speeds) in the x and z axis
    public void RandimiseAllTarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }

    void ResetToTrack()
    {
        int pointToGoTo = nextCheckpoint - 1;       // substract one from this checkpoint to move to the previous one
        if (pointToGoTo < 0)
        {
            // if this is the first checkpoint, substract one from the length of the checkpoint array and store it in the pointtogo var
            pointToGoTo = RaceManager.instance.allCheckpoints.Length - 1;
        }
        transform.position = RaceManager.instance.allCheckpoints[pointToGoTo].transform.position;         // store that position to the tranform.position var
        RB.transform.position = transform.position;                                                       // move the sphere to that position 

        RB.velocity = Vector3.zero;     // reset the speed of the sphere and the car to 0
        speedInput = 0;     // reset the speed to 0
        turnInput = 0;      // reset the turning to 0
        grounded = true;
    }
}
