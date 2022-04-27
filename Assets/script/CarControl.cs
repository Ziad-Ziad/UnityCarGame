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
    public float groundRayLength = 1f; // if the distance between the car and the ground is bigger than this so it is considered to be in the air
    float dragOnGround;
    public float gravityMode = 10f;
    public AudioSource engineSound;    // attaching the engine sound
    public int nextCheckpoint;
    public int currentLap = 1;
    public float lapTime, bestLapTime;

    // Start is called before the first frame update
    void Start()
    {
        RB.transform.parent = null;
        dragOnGround = RB.drag;
        UIControl.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;     // Modifying the value of lap counter indicator

    }

    private void Update()
    {
        lapTime += Time.deltaTime;  // increment the time by the previous one
        var timespan = System.TimeSpan.FromSeconds(lapTime);

        //declarng the format of the best lap time and assigning it 
        UIControl.instance.currentLapText.text = string.Format("{0:00}m{1:00}.{2:000}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

        engineSound.pitch = 1f + (RB.velocity.magnitude / maxSpeed) * 2f;       // Increasing the pitch when the velociy increases to make it more realistic
        // print(engineSound.pitch);
    }
    // Update is called once per frame
    void FixedUpdate()
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

        // rotate the car only on the ground layer
        if (grounded && Input.GetAxis("Vertical") != 0)
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
    }

    public void LapCompleted()
    {
        currentLap++;


        // if the current lap time is less than the bestlap time sp the current one when we start the game the lap time will be the bestlaptime
        if (lapTime < bestLapTime || bestLapTime == 0) // || - or, && - and
        {
            bestLapTime = lapTime;
        }
        lapTime = 0f;

        var timespan = System.TimeSpan.FromSeconds(bestLapTime);
        UIControl.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

        UIControl.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;     // Modifying the value of lap counter indicator


    }
}
