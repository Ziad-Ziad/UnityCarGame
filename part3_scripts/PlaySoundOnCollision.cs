using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{

    public AudioSource soundHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // play the sound when the sphere collide with the cube
        if(collision.gameObject.layer != 6)
        {
            soundHit.Stop();
            soundHit.pitch = Random.Range(1f, 2f);      // Defining a random pitch degree each time of the collision
            soundHit.Play();
        }

    }
}
