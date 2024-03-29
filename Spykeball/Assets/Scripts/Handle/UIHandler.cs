using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : Handler
{

    // delegates and events
    public delegate void onTimeRunOut();
    public static event onTimeRunOut doOnTimeRunOut;

    [SerializeField]
    protected PlayerSpyke player;

    [SerializeField]
    protected PlayerBall ball;

    [SerializeField]
    protected Text scoreboard;

    public float timeAttackLimit;
    public GameObject panelMainMenu, panelRestartMenu;
    public Text[] timerText;

    protected int currentScore = 0, gameTimerSecs = 0;
    protected int[] timers = new int[] {0,0,0};



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
    protected override void doOnRestartHandler() {

        if (Mode == Modes.Survival) {

        }
        else if (Mode == Modes.TimeAttack) {
            setTimerTexts();
            startTimer();
        }

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

    /// <summary>
    /// 
    /// Timer functions here
    /// 
    /// </summary>

    // Set timer texts
    protected void setTimerTexts() {
        int mintext = (int) timeAttackLimit / 60;

        // set the timer
        gameTimerSecs = (int)timeAttackLimit;

        // set the timer counters
        timers[0] = 9;
        timers[1] = 0;
        timers[2] = mintext;

        timerText[0].text = "00"; // msecs
        timerText[1].text = "00"; // secs
        timerText[2].text = mintext.ToString("D2");
    }

    // Set timer 
    protected void updateTimerTexts(int index, string value) {
        if (index >= timerText.Length) return;
        timerText[index].text = value;
    }

    // start the timer here
    protected void startTimer() {
        StartCoroutine("countDownMsecs");
    }

    // countDown milliseconds
    IEnumerator countDownMsecs() {
        while (gameTimerSecs > 0) {        
            timers[0]--; 
            if (timers[0] < 0) {
                gameTimerSecs--;
                timers[1]--; // update the seconds
                if (timers[1] < 0) {
                    timers[1] = 59;
                    timers[2]--; // update the seconds
                    if (timers[2] < 0) {
                        timers[2] = 59; 
                        // end game here
                    }
                    updateTimerTexts(2, timers[2].ToString("D2"));
                }
                updateTimerTexts(1, timers[1].ToString("D2"));
                timers[0] = 9;
            }
            updateTimerTexts(0, timers[0].ToString("D2"));
            yield return new WaitForSeconds(0.1f); // return every 10 msecs
        }

        if (gameTimerSecs <= 0) {
            timers[0] = 0;
            updateTimerTexts(0, timers[0].ToString("D2")); // fix display to 0
            Debug.Log("Timer has ended");
            doOnTimeRunOut();
        }
        yield return true;
    }

}
