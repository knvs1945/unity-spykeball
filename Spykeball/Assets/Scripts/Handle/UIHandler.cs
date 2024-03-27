using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : Handler
{

    [SerializeField]
    protected PlayerSpyke player;

    [SerializeField]
    protected PlayerBall ball;

    [SerializeField]
    protected Text scoreboard;

    public GameObject panelMainMenu, panelRestartMenu;

    protected int currentScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        resetUIStats();
        registerEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register and unregister events
    protected void registerEvents() {
        // register events from the game objects
        if (player != null) {
            player.doOnHitBall += updateScore;
        }
        if (ball != null) {
            ball.doOnHitTarget += updateScore;
        }

    }

    protected void unRegisterEvents() {
       if (player != null) {
            player.doOnHitBall -= updateScore;
        }
        if (ball != null) {
            ball.doOnHitTarget -= updateScore;
        }
    }

    // restart handler and elements here
    protected override void doOnRestartHandler(string gameMode) {
        Debug.Log("Restarting UI handler");
        resetUIStats();
        panelMainMenu.SetActive(false);
        panelRestartMenu.SetActive(false);
    }

    // show the end game panel
    public void showEndGamePanel() {
        panelRestartMenu.SetActive(true);
    }

    // Update the score here
    protected void updateScore(int score) {
        currentScore += score;
        scoreboard.text = currentScore.ToString();
    }

    protected void resetUIStats() {
        currentScore = 0;
        scoreboard.text = currentScore.ToString();
    }

}
