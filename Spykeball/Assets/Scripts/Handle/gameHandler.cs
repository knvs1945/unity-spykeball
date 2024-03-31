using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Handler
{
    
    [SerializeField]
    protected PlayerHandler playerHandle;

    [SerializeField]
    protected UIHandler UIHandle;

    [SerializeField]
    protected TargetHandler targetHandle;
    private bool stagePrepFlag;

    // Start is called before the first frame update

    void Start()
    {
        // fix framerate to default FPS (30);
        Application.targetFrameRate = FPS;

        // test setBPM
        // RhythmHandler.setBPM(60);

        // move this somewhere else once startLevel works
        freezeGame(true);
        doReturnToMainMenu("");
    }

    void Awake() 
    {
        // restartAllHandlers();
        registerEvents();
    }

    // Update is called once per frame
    void Update()
    {
        checkExitGame();
    }
    
    
    // get gameState
    public static states getGameState() {
        return gameState;
    }

    public static bool isGamePaused() {
        return pauseGame;
    }

    // setting to true will freeze the game, and false unfreezes it
    protected void freezeGame(bool val) {
        Time.timeScale = val ? 0 : 1;
    }

    public void checkExitGame() {
        if (Input.GetKeyDown("r")) restartAllHandlers(1, ""); // restart whatever game mode they were in
        if (Input.GetKeyDown("escape")) Application.Quit();
    }

    // register events
    protected void registerEvents() {
        
        // register events from MainMenu & Restart screen
        ButtonMainMenu.doOnStartGame += restartAllHandlers;
        ButtonMainMenu.doOnReturnToMain += doReturnToMainMenu;
        ButtonMainMenu.doOnUnpauseGame += doOnGamePaused;

        Handler.doOnGameOver += GameEnded;
        UIHandler.doOnTimeRunOut += GameEnded;
        PlayerHandler.doOnPlayerPaused += doOnGamePaused;

    }

    protected void doReturnToMainMenu(string evt) {
        GameUnit.gameState = 0;
        doOnGamePaused(false);
        UIHandle.returnToMainMenu();
        playerHandle.returnToMainMenu();
        targetHandle.returnToMainMenu();
    }

    protected void restartAllHandlers(int mode, string gameType) {
        switch(gameType) {
            case "survival":
                Mode = Modes.Survival;
                break;
            case "time attack":
                Mode = Modes.TimeAttack;
                break;
        }

        playerHandle.restartHandler();
        UIHandle.restartHandler();
        targetHandle.restartHandler();
        freezeGame(false);
        gameLevel = 0;
        GameUnit.gameState = 1; // inform the units that we are in game mode
    }

    /* 
    *
    *   Start the stage sequences
    *
    */
    public bool startLevel() {
        stagePrepFlag = false;
        return stagePrepFlag;
    }

    // pause the game 
    protected void doOnGamePaused(bool paused) {
        Debug.Log("Pausing the game...");
        GameUnit.isGamePaused = paused;
        playerHandle.pauseHandler(paused);
        targetHandle.pauseHandler(paused);
        UIHandle.pauseHandler(paused);
    }

    // game end sequence
    protected void GameEnded() {
        freezeGame(true);
        UIHandle.showEndGamePanel();
    }

}
