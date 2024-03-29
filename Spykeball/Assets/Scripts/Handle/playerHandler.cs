﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Handler for players
public class PlayerHandler : Handler
{

    public Transform[] playerSpawns;
    protected Slider playerHP;
    protected Vector3 HPBarPos; 

    [SerializeField]
    protected PlayerUnit playerObj;
    protected Transform currentPlayerSpawn;

    [SerializeField]
    protected PlayerBall ball;

    // Player's controls
    protected PlayerControls controlPlayer1;

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

        // get the HP bar of the player
        // playerHP = GameObject.Find("PlayerHP").GetComponent<Slider>();
        // playerHP.value = playerObj.Player.HP;
        // HPBarPos = playerHP.transform.localPosition;

        do {
            if (playerObj != null) {
                // register to event when player gets damaged
                // PlayerUnit.updatePlayerHPBar += updatePlayerHPBar;
                playerObj.Player.IsControlDisabled = false;
                Debug.Log("Registering updatePlayerHPBar event...");
            }
        } while (playerObj == null);

        registerEvents();
    }

    // register and deregister events here
    protected void registerEvents() {
        if (ball != null) {
            ball.doOnNoMoreLives += doOnPlayerBallGone;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Restart the game sets
    protected override void doOnRestartHandler(string gameType) {
        Debug.Log("Restarting player handler");
        playerObj.restartUnit(gameType);       
        ball.restartUnit(gameType);
        //ball.GetComponent<GameObject>().SetActive(true);
        ball.gameObject.SetActive(true);
    }

    // update HP Bar after getting damaged
    protected void updatePlayerHPBar() {
        if (playerObj.Player.IsDamageShldActive) {
            playerHP.value = playerObj.Player.HP;
            shakeHPBar();
        }
    }

    // shake the HP bar when damaged
    protected void shakeHPBar() {
        StartCoroutine(shakeCoroutine(playerObj.Player.base_DMGdelay, 20f));
        playerHP.transform.localPosition = HPBarPos;
    }

    // coroutine that handles the HPbar shake
    private IEnumerator shakeCoroutine(float duration, float magnitude) {
        Vector3 originalPos = playerHP.transform.localPosition;
        float elapsed = 0.0f;
        float shakeDirection = -1f;

        while (elapsed < duration) {
            // float y = Random.Range(-1f, 1f) * magnitude;
            float y = shakeDirection * magnitude;
            playerHP.transform.localPosition = new Vector3(originalPos.x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            if (magnitude > 0) {
                magnitude -= 0.5f;
                shakeDirection *= -1f; // flip direction
            }
            // else magnitude = 0;
            yield return null;
        }

        playerHP.transform.localPosition = originalPos;
    }

    /* 
    *
    *   Start stage sequences 
    *
    */
    public bool startStageSequence() {
        
        // check if player spawns are present and assign the current player spawn to the first one if not yet done
        if (playerSpawns.Length > 0) {
            if (!currentPlayerSpawn) currentPlayerSpawn = playerSpawns[0];
        }
        else {
            // unassigned player spawn array will return false
            Debug.Log("currentPlayerSpawn is currently unassigned.");
            return false;
        }

        // start the animation for the intro sequence and transfer the player to the playerSpawn
        if (playerObj) {
            playerObj.Player.IsControlDisabled = true;
            playerObj.Player.transform.position = currentPlayerSpawn.position;
            StartCoroutine(prepPlayerForStage());   // countdown before starting the stage
            Debug.Log("Player now able to use controls");
        }
        
        return true;
    }

    // IEnumerator for setting up the stage for the player
    IEnumerator prepPlayerForStage() {

        // stageIntroTimer is a static value 
        float Timer = stageIntroTimer;
        
        while (Timer >= 0) {
            yield return new WaitForSeconds(1);
            Timer--;            
        }
        
        playerObj.Player.IsControlDisabled = false;

        // get the player object ang transfer it to the current player spawn
    }

    // report game over when the ball has no more lives
    protected void doOnPlayerBallGone() {
        Debug.Log(" Player ball is now gone ");
        ball.gameObject.SetActive(false);
        doOnGameOver();
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

}

