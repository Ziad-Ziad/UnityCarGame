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


    // Start is called before the first frame update
    void Start()
    {
        RB.transform.parent = null;
        dragOnGround = RB.drag;
    }

    private void Update()
    {
        engineSound.pitch = 1f + (RB.velocity.magnitude / maxSpeed) * 2f;       // Increasing the pitch when the velociy increases to make it more realistic
        print(engineSound.pitch);
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
}
