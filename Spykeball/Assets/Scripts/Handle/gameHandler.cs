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

    [SerializeField]
    protected SoundHandler soundHandle;

    [SerializeField]
    protected TransitionHandler transitionHandle;
    
    private string currentGameType;
    private int currentMode;
    private bool stagePrepFlag;

    // Start is called before the first frame update

    void Start()
    {
        // fix framerate to default FPS (30);
        Application.targetFrameRate = FPS;

        // move this somewhere else once startLevel works
        freezeGame(true);
        doReturnToMainMenu("");
    }

    void Awake() 
    {
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
        if (Input.GetKeyDown("escape")) {
            Application.Quit();
        }
    }

    // register events
    protected void registerEvents() {
        
        // register events from MainMenu & Restart screen
        // ButtonMainMenu.doOnStartGame += restartAllHandlers;
        ButtonMainMenu.doOnStartGame += doOnStartTransition;
        ButtonMainMenu.doOnReturnToMain += doReturnToMainMenu;
        ButtonMainMenu.doOnUnpauseGame += doOnGamePaused;

        // register transition event
        TransPanel.doOnTransitionEvent += doOnTransition;

        Handler.doOnGameOver += GameEnded;
        UIHandler.doOnTimeRunOut += GameEnded;
        UIHandler.doOnGetNewControls += doOnGetNewControls;
        PlayerHandler.doOnPlayerPaused += doOnGamePaused;
        
    }

    protected void doReturnToMainMenu(string evt) {
        GameUnit.gameState = 0;
        gameState = states.MainMenu;
        doOnGamePaused(false);
        
        // get the current controls and volumes used by the player
        UIHandle.setCurrentControls(playerHandle.ControlPlayer1);
        UIHandle.setCurrentVolumes(soundHandle.getVolumeValues());

        UIHandle.returnToMainMenu();
        playerHandle.returnToMainMenu();
        targetHandle.returnToMainMenu();
        soundHandle.returnToMainMenu();
    }

    protected void doOnGetNewControls() {
        playerHandle.ControlPlayer1 = UIHandle.getNewControls();
    }

    // listen to the start game button
    protected void doOnStartTransition(int mode, string gameType) {
        freezeGame(false);
        currentMode = mode;
        currentGameType = gameType;
        transitionHandle.startStateTransition();
    }

    // listen to the transition panel sliding in
    protected void doOnTransition(string eventName) {
        
        switch(eventName) {
            case "prepareGame": freezeGame(true);
                                restartAllHandlers(currentMode, currentGameType);
                                break;
        }
    }

    // initiate the preparation of the other handlers for the game
    protected void restartAllHandlers(int mode, string gameType) {
        
        GameUnit.gameState = 1; // inform the units that we are in game mode
        GameUnit.isGamePaused = false; // make sure the pause state is also restarted
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
        gameLevel = 0;
        soundHandle.restartHandler();
        freezeGame(false);
        transitionHandle.endStateTransition();
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
