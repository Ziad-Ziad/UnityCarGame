using System.Collections;
using System.Collections.Generic;
using  UnityEngine ;

public class CarControl : MonoBehaviour
{
    public float maxSpeed = 30;     // max speed of the car is 30         
    public Rigidbody RB;            //  declaring the rigid body of the car
    float speedInput;
    public float forwardAccel = 10f, reverseAccel = 5f;
    float turnStrength = 180f;
    float turnInput;
    public Transform leftfrontWheel, rightfrontWheel;       // variables of rotating the car right or left
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
        // once the game starts remove the "sphere / rigid body" from the racing car
        RB.transform.parent = null;
        // at the start the drag is equal to the default one
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
            normalTarget = hit.normal;  // the target is equal to the hit normal (Perpendicular)
        }

        speedInput = 0f;               // the initial speed of the car is set to 0

        //  moving the car forward if the up arrow or the "w" button is clicked
        if (Input.GetAxis("Vertical") > 0)
        {
            // accelerate the car as the button is being pressed
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        }

        //  moving the car backward if the down arrow or the "s" button is clicked
        if (Input.GetAxis("Vertical") < 0)
        {
            // accelerate the car as the button is being pressed
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        // retreiving the inout of the horizontal axis
        turnInput = Input.GetAxis("Horizontal");

        // rotate the car only on the ground layer && The rotation of the car is only possible when it is moving forward or backwards ("vertical") != 0
        if (grounded && Input.GetAxis("Vertical") != 0)
        {
            // rotating the car in the y axis
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (RB.velocity.magnitude / maxSpeed), 0));
        }

        // rotating the car's front-left wheel
        leftfrontWheel.localRotation = Quaternion.Euler(leftfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), leftfrontWheel.localRotation.eulerAngles.z);
        
        // rotating the car's front-right wheel
        rightfrontWheel.localRotation = Quaternion.Euler(rightfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), rightfrontWheel.localRotation.eulerAngles.z);

        // rotate the car to match the normal
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation; // Allign the car along the slopped plane 

        }
        // accelerate the car only on the ground layer
        if (grounded)
        {
            // adding force to the car to make it move
            RB.AddForce(transform.forward * speedInput * 10000f);
            // when the car on the ground , the rigid body drag is the default one
            RB.drag = dragOnGround;
        }
        else
        {   
            // whe the car is not on the groud, the drag is decreased
            RB.drag = 0.1f;     // decreasing the drag value when the car is on the air to make it move 
            RB.AddForce(-Vector3.up * gravityMode * 100f); //increasing the gravity to pull the car to the ground faser to make it more realistic
        }

        // moving the whole car according to the rigid body position  and the transform method
        transform.position = RB.position;

        // print(RB.velocity.magnitude);

        // limiting the car's speed
        if (RB.velocity.magnitude > maxSpeed)
        {
            RB.velocity = RB.velocity.normalized * maxSpeed; // 0...55 -> (0...1) * MAXSpeed -> 0...30 limited for the speed cars
        }

    }
}