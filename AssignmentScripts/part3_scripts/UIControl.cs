using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIControl : MonoBehaviour
{
    
    public static UIControl instance;       // accessing the static memebers of the object
    public TMP_Text currentLapText, bestLapTimeText, lapCounterText; // text variables

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
        
    }
}
