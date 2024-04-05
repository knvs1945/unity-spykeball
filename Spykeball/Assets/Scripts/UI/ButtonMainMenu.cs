using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for buttons in the Main Menu 
public class ButtonMainMenu : MonoBehaviour
{
    public delegate void onMainMenuButton(int mode, string gametype);
    public delegate void onReturnToMain(string evt);
    public delegate void onUnpauseGame(bool paused);
    public delegate void onOpenSettings(bool open);
    
    public static event onMainMenuButton doOnStartGame;
    public static event onReturnToMain doOnReturnToMain;
    public static event onUnpauseGame doOnUnpauseGame;
    public static event onOpenSettings doOnOpenSettings;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // starts the game
    public void btStartGame(string gameMode) {
        Debug.Log("Start game Button Clicked");
        doOnStartGame(1, gameMode);
    }

    // returns to the main menu
    public void btReturnToMain(string evt) {
        Debug.Log("Returning to Main menu");
        doOnReturnToMain(evt);   
    }

    // unpauses the game if unpause is clicked
    public void btUnpauseGame(bool state) {
        doOnUnpauseGame(state);
    }

    public void btOpenSettings(bool open) {
        doOnOpenSettings(open);
    }
}
