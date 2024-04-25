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

    [SerializeField]
    protected EnvHandler envHandle;
    
    private string currentGameType;
    private int currentMode;
    private bool stagePrepFlag;

    // Start is called before the first frame update

    void Start()
    {
        // fix framerate to default FPS (30);
        Application.targetFrameRate = FPS;

        // move this somewhere else once startLevel works
        // freezeGame(true);
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
        // ButtonMainMenu.doOnReturnToMain += doReturnToMainMenu;
        ButtonMainMenu.doOnStartGame += doOnStartTransition;
        ButtonMainMenu.doOnReturnToMain += doOnReturnTransition;
        ButtonMainMenu.doOnUnpauseGame += doOnGamePaused;

        // register transition event
        TransPanel.doOnTransitionEvent += doOnTransition;

        Handler.doOnGameOver += GameEnded;
        UIHandler.doOnTimeRunOut += GameEnded;
        UIHandler.doOnGetNewControls += doOnGetNewControls;
        PlayerHandler.doOnPlayerPaused += doOnGamePaused;
        PlayerHandler.doOnIntroSequenceDone += startRound;
        
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
        transitionHandle.setNextTransition("prepareGame");
        transitionHandle.startStateTransition();
    }

    // listen to return main menu button
    protected void doOnReturnTransition(string gameType) {
        currentGameType = gameType;
        transitionHandle.setNextTransition("startReturnToMainMenu");
        transitionHandle.startStateTransition();
    }

    // listen to the transition panel sliding in
    protected void doOnTransition(string eventName) {
        
        switch(eventName) {
            case "prepareGame": // freezeGame(true);
                                restartAllHandlers(currentMode, currentGameType);
                                break;
            case "startRoundIntro": startRoundIntro();
                                    break;
            case "startReturnToMainMenu": doReturnToMainMenu("");
                                          transitionHandle.setNextTransition("returnToMainMenu");
                                          transitionHandle.endStateTransition();
                                          break;
            case "returnToMainMenu":    break;
            
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

        UIHandle.restartHandler();
        soundHandle.restartHandler();
        envHandle.restartHandler();
        playerHandle.restartHandler();
        
        gameLevel = 0;
        freezeGame(false);

        // set the next transition event to start round
        transitionHandle.setNextTransition("startRoundIntro");
        transitionHandle.endStateTransition();
    }

    // initiate the round intro start after the transition
    protected void startRoundIntro() {
        playerHandle.startGameIntro();
    }

    // officially starts the round after the intro sequence is done
    protected void startRound() {
        targetHandle.restartHandler();
        if (Mode == Modes.TimeAttack) {
            UIHandle.restartTimeAttackTimer();
        }
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
        GameUnit.isGamePaused = paused;
        playerHandle.pauseHandler(paused);
        targetHandle.pauseHandler(paused);
        UIHandle.pauseHandler(paused);
    }

    // game end sequence
    protected void GameEnded() {
        UIHandle.showEndGamePanel();
    }

}
