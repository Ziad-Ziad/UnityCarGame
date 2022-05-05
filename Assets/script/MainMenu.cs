using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;      // connecting the scene libraries to switch and manipulate them

public class MainMenu : MonoBehaviour
{
    public string levelToLoad;           //  the game  scene
    public string mainMenu;              // the main menu scene
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);        // load the game when the start game function is invoked
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenu);           // load the main menu scene when the back to menu function is invked
        Time.timeScale = 1;
    }

    public void QuitGame()                          // quit the application (game) once the quitgame function is invoked
    {
        Application.Quit();
        print("Game Qiuit");
    }
}