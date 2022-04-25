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


    // Start is called before the first frame update
    void Start()
    {
        RB.transform.parent = null;
    }


    // Update is called once per frame
    void FixedUpdate()
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
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (RB.velocity.magnitude / maxSpeed), 0));
        }

        leftfrontWheel.localRotation = Quaternion.Euler(leftfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), leftfrontWheel.localRotation.eulerAngles.z); 
        rightfrontWheel.localRotation = Quaternion.Euler(rightfrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheeltuen), rightfrontWheel.localRotation.eulerAngles.z);

        RB.AddForce(transform.forward * speedInput * 10000f);
        transform.position = RB.position;

        if (RB.velocity.magnitude > maxSpeed)
        {
            RB.velocity = RB.velocity.normalized * maxSpeed; // 0...55 -> (0...1) * MAXSpeed -> 0...30 limited for the speed cars
        }
        print(RB.velocity.magnitude);

    }
}
