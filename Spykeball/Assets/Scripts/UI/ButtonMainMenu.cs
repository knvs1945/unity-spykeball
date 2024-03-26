using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for buttons in the Main Menu 
public class ButtonMainMenu : MonoBehaviour
{
    public delegate void onMainMenuButton(int mode, string gametype);
    public static event onMainMenuButton doOnStartGame;

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
}
