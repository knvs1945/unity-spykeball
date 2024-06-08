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
    public delegate void onOpenScoreboard(bool open);
    
    public static event onMainMenuButton doOnStartGame;
    public static event onReturnToMain doOnReturnToMain;
    public static event onUnpauseGame doOnUnpauseGame;
    public static event onOpenSettings doOnOpenSettings;
    public static event onOpenScoreboard doOnOpenScoreboard;

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
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnStartGame(1, gameMode);
    }

    // returns to the main menu
    public void btReturnToMain(string evt) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnReturnToMain(evt);
    }

    // unpauses the game if unpause is clicked
    public void btUnpauseGame(bool state) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnUnpauseGame(state);
    }

    public void btOpenSettings(bool open) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnOpenSettings(open);
    }

    public void btOpenScoreboard(bool open) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnOpenScoreboard(open);
    }
}
