using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIControl : MonoBehaviour
{
    
    public static UIControl instance;       // accessing the static memebers of the object

    /* declaring the ui variables
     *          the lap's number -the best lab for the player- current lap timing - the current rank of the player's car in the race  - the starting counter var - 
     *          - ui var for displaying the "GO" word after the counter finishes  - the position of the player in the end of the race
     */
    public TMP_Text currentLapText, bestLapTimeText, lapCounterText, positionText, countDownText, goText, raceResultText; // text variables
    public GameObject resultScreen, pauseScreen;

    public bool isPaused;       // stroing the state if the game wether it is paused or not3

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))   // if the esc button is pressed, invoke the pauseUnpause function
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;                   // invert the ispaused value
        pauseScreen.SetActive(isPaused);        // activate the game object pause screen
        if (isPaused)       
        {
            Time.timeScale = 0;                 // stop the game
        }
        else
        {
            Time.timeScale = 1;                 // resume the game
        }
    }
}
