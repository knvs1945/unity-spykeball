using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler for players
/// </summary>
public class PlayerHandler : Handler
{

    // events and delegates
    public delegate void onPausePressed(bool paused);
    public delegate void onIntroSequenceDone();
    public static event onPausePressed doOnPlayerPaused;
    public static event onIntroSequenceDone doOnIntroSequenceDone;

    protected Slider playerHP;
    protected Vector3 HPBarPos; 

    [SerializeField]
    protected PlayerUnit playerObj;

    [SerializeField]
    protected PlayerBall ball;

    // Player's controls
    protected PlayerControls controlPlayer1;

    // getters and setters
    public PlayerControls ControlPlayer1 {
        get { return controlPlayer1; }
        set {
            controlPlayer1 = value;
            playerObj.Controls = controlPlayer1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // create default controls for player
        controlPlayer1 = new PlayerControls(
            "w", "s", "a", "d",
            "space", "i", "shift", "p"
        );
        
        // assign the control set to player 1
        playerObj.Controls = controlPlayer1;
    }

    void Awake() {
        registerEvents();
    }

    // register and deregister events here
    protected void registerEvents() {
        if (ball != null) {
            ball.doOnNoMoreLives += doOnPlayerBallGone;
        }
        if (playerObj != null) {
            PlayerUnit.doOnPausePressed += playerPressedPause;
            PlayerUnit.doOnIntroSequenceDone += introSequenceDone;
        }

        UIHandler.doOnTimeRunOut += doOnTimeRunOut;
    }


    public override void returnToMainMenu() {
        // hide the current target
        ball.deactivate();
        ball.gameObject.SetActive(false);
        // playerObj.IsControlDisabled = true;
        playerObj.Player.IsControlDisabled = true;
    }

    // starts the game start intro;
    public void startGameIntro() {

        string mode = "Survival";
        if (Mode == Modes.Survival) {
            mode = "Survival";       
        }
        else if (Mode == Modes.TimeAttack) {
            mode = "Time Attack";       
        }
        
        EnvHandler.Instance.introDoorOpen();
        playerObj.gameObject.SetActive(true);
        playerObj.restartUnit(mode);       
    }

    // Restart the game sets
    protected override void doOnRestartHandler() {
        // hide the player entries first
        ball.gameObject.SetActive(false);
        playerObj.gameObject.SetActive(false);
    }

    // paused player objects
    protected override void doOnPauseHandler(bool state) {
    }

    // fire doOnintroSequenceDone event
    protected void introSequenceDone() {
        string mode = "Survival";
        if (Mode == Modes.Survival) {
            mode = "Survival";       
        }
        else if (Mode == Modes.TimeAttack) {
            mode = "Time Attack";       
        }
        ball.gameObject.SetActive(true);
        ball.restartUnit(mode);

        if (playerObj != null) playerObj.Player.IsControlDisabled = false;
        doOnIntroSequenceDone();
    }


    // report game over when the ball has no more lives
    protected void doOnPlayerBallGone() {
        playerObj.Player.startOutroanimation();
        playerObj.Player.IsControlDisabled = true;
        ball.deactivate();
        gameState = states.GameEnd;
        doOnGameOver();
    }

    protected void doOnTimeRunOut() {
        playerObj.Player.startOutroanimation();
        playerObj.Player.IsControlDisabled = true;
        ball.deactivate();
        gameState = states.GameEnd;
    }

    // report game pause entries from player unit
    protected void playerPressedPause(bool state) {
        doOnPlayerPaused(state);
    }
}

// class to contain player controls
public class PlayerControls {
    protected string moveUp, moveDown, moveLeft, moveRight, attack, defend, dodge, pause;

    // constructor
    public PlayerControls(
        string _up, string _down, string _left, string _right, 
        string _attack, string _defend, string _dodge, string _pause
    ) {
        moveUp = _up;
        moveDown = _down;
        moveLeft = _left;
        moveRight = _right;
        attack = _attack;
        defend = _defend;
        dodge = _dodge;
        pause = _pause;
    }

    // overridden constructor
    public PlayerControls(
        string[] controlSet
    ) {
        if (controlSet.Length < 8) {
            throw new ArgumentException("Invalid controlSet property. Must have 8 elements to initialize a PlayerControl class");
        }
        moveUp = controlSet[0];
        moveDown = controlSet[1];
        moveLeft = controlSet[2];
        moveRight = controlSet[3];
        attack = controlSet[4];
        defend = controlSet[5];
        dodge = controlSet[6];
        pause = controlSet[7];
    }

    // getters & setters
    public string MoveUp {
        get { return moveUp; }
        set { moveUp = value; }
    }
    public string MoveDown {
        get { return moveDown; }
        set { moveDown = value; }
    }
    public string MoveLeft {
        get { return moveLeft; }
        set { moveLeft = value; }
    }
    public string MoveRight {
        get { return moveRight; }
        set { moveRight = value; }
    }
    public string Attack {
        get { return attack; }
        set { attack = value; }
    }
    public string Defend {
        get { return defend; }
        set { defend = value; }
    }
    public string Dodge {
        get { return dodge; }
        set { dodge = value; }
    }
    public string Pause {
        get { return pause; }
        set { pause = value; }
    }

    public string[] getControlSet() {
        return new string [] {moveUp, moveDown, moveLeft, moveRight, attack, defend, dodge, pause};
    }

    protected void checkForSpecialKeys(string controlKey) {
        
    }
}

// class to contain player controls
/*
public class PlayerControls {
    protected KeyCode moveUp, moveDown, moveLeft, moveRight, attack, defend, dodge, pause;

    // constructor
    public PlayerControls(
        KeyCode _up, KeyCode _down, KeyCode _left, KeyCode _right, 
        KeyCode _attack, KeyCode _defend, KeyCode _dodge, KeyCode _pause
    ) {
        moveUp = _up;
        moveDown = _down;
        moveLeft = _left;
        moveRight = _right;
        attack = _attack;
        defend = _defend;
        dodge = _dodge;
        pause = _pause;
    }

    // overridden constructor
    public PlayerControls(
        KeyCode[] controlSet
    ) {
        if (controlSet.Length < 8) {
            throw new ArgumentException("Invalid controlSet property. Must have 8 elements to initialize a PlayerControl class");
        }
        moveUp = controlSet[0];
        moveDown = controlSet[1];
        moveLeft = controlSet[2];
        moveRight = controlSet[3];
        attack = controlSet[4];
        defend = controlSet[5];
        dodge = controlSet[6];
        pause = controlSet[7];
    }

    // getters & setters
    public string MoveUp {
        get { return moveUp; }
        set { moveUp = value; }
    }
    public string MoveDown {
        get { return moveDown; }
        set { moveDown = value; }
    }
    public string MoveLeft {
        get { return moveLeft; }
        set { moveLeft = value; }
    }
    public string MoveRight {
        get { return moveRight; }
        set { moveRight = value; }
    }
    public string Attack {
        get { return attack; }
        set { attack = value; }
    }
    public string Defend {
        get { return defend; }
        set { defend = value; }
    }
    public string Dodge {
        get { return dodge; }
        set { dodge = value; }
    }
    public string Pause {
        get { return pause; }
        set { pause = value; }
    }

    public string[] getControlSet() {
        return new string [] {moveUp, moveDown, moveLeft, moveRight, attack, defend, dodge, pause};
    }

    protected void checkForSpecialKeys(string controlKey) {
        
    }
}*/

