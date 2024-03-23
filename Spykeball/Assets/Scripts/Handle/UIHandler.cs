using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : Handler
{

    [SerializeField]
    protected PlayerSpyke player;

    [SerializeField]
    protected Text scoreboard;

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
        if (player != null) {
            player.doOnHitBall += updateScore;
        }
    }

    protected void unRegisterEvents() {
       if (player != null) {
            player.doOnHitBall -= updateScore;
        }
    }

    // Update the score here
    protected void updateScore(int score) {
        currentScore += score;
        scoreboard.text = currentScore.ToString();
    }

    protected void resetUIStats() {
        currentScore = 0;
    }
}
