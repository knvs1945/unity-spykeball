using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler for the User Interface
/// </summary>
public class UIHandler : Handler
{

    // delegates and events
    public delegate void onTimeRunOut();
    public delegate void onGetNewControls();

    public static event onTimeRunOut doOnTimeRunOut;
    public static event onGetNewControls doOnGetNewControls;

    [SerializeField]
    protected PlayerSpyke player;

    [SerializeField]
    protected PlayerBall ball;

    [SerializeField]
    protected Text scoreboard;

    [SerializeField]
    protected Text targetText;

    public float timeAttackLimit;
    public GameObject panelMainMenu, panelRestartMenu;
    public GameObject panelTimer, panelLives, panelScore, panelTargets, panelPauseMenu, panelControls; // ingame panels
    public Image[] livesCount;
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
            ball.doOnLivesLeft += updateLives;
            ball.doOnHitTarget += updateScore;
            ball.doOnHitFloor += updateScore;
            ball.doOnHitTarget += updateGameLevel;
        }

        ButtonMainMenu.doOnOpenSettings += openSettingsPanel;
        ControlPanel.doOnCloseSettings += saveSettingsAndClose;
    }

    protected void unRegisterEvents() {
       if (player != null) {
            player.doOnHitBall -= updateScore;
        }
        if (ball != null) {
            ball.doOnLivesLeft -= updateLives;
            ball.doOnHitTarget -= updateScore;
            ball.doOnHitFloor -= updateScore;
        }
    }

    // restart handler and elements here
    protected override void doOnRestartHandler() {

        // enable needed panels and set up the UI
        if (Mode == Modes.Survival) {
            panelScore.SetActive(true);
            panelLives.SetActive(true);
            panelTimer.SetActive(false);
            updateLives(4);
        }
        else if (Mode == Modes.TimeAttack) {
            panelScore.SetActive(true);
            panelLives.SetActive(false);
            panelTimer.SetActive(true);
            setTimerTexts();
            startTimer();
        }

        panelTargets.SetActive(true);
        panelMainMenu.SetActive(false);
        panelRestartMenu.SetActive(false);
        panelPauseMenu.SetActive(false);
        panelControls.SetActive(false);
        resetUIStats();   

        // game is starting, set the game state as needed
        gameState = states.inStage;
        
    }

    // paused UI objects
    protected override void doOnPauseHandler(bool state) {
        if (state) {
            // stop the timer if it is counting
            panelPauseMenu.SetActive(true);
        }
        else {
            panelPauseMenu.SetActive(false);
        }
    }
    
    // show the main menu panel coming from other buttons
    public override void returnToMainMenu() {
        // hide the ingame panels
        panelScore.SetActive(false);
        panelLives.SetActive(false);
        panelTimer.SetActive(false);
        panelTargets.SetActive(false);

        panelMainMenu.SetActive(true);
        panelRestartMenu.SetActive(false);
        panelPauseMenu.SetActive(false);
        panelControls.SetActive(false);
    }

    // show the end game panel
    public void showEndGamePanel() {
        panelRestartMenu.SetActive(true);
        gameState = states.GameEnd;
    }

    // show the controls panel. Set to true if open and false if close
    public void openSettingsPanel(bool open) {
        
        if (open) {
            if (gameState == states.MainMenu) {
                panelMainMenu.SetActive(false); // hide the main menu if entering settings
            }
        }
        else {
            if (gameState == states.MainMenu) {
                panelMainMenu.SetActive(true); // show the main menu if exiting settings
            }
        }

        panelControls.SetActive(open);
    }

    // close the settings panel with or without saving the changes
    public void saveSettingsAndClose(bool saveSettings) {
        if (!saveSettings) {
            // do nothing and close
            openSettingsPanel(false); // close the control panel;
        }
        else {
            // save the controls here before closing 
            doOnGetNewControls();
            openSettingsPanel(false); // close the control panel;
        }
    }

    // set the controls from the player to display in the control panel
    public void setCurrentControls(PlayerControls controls) {
        panelControls.GetComponent<ControlPanel>().setCurrentControlUI(controls);
    }

    public PlayerControls getNewControls() {
        PlayerControls newControl = panelControls.GetComponent<ControlPanel>().getCurrentControl();
        return newControl;
    }

    // Update the score here
    protected void updateScore(int score, int time = 0) {
        currentScore += score;
        scoreboard.text = currentScore.ToString();
        if (Mode == Modes.TimeAttack) addTime(time);
    }

    protected void updateLives(int lives) {
        bool active = true;
        int currentLives = Mathf.Abs(lives - 1);
        if (lives <= 0) return;

        for (int i = 0; i < livesCount.Length; i++ ) {
            if (i >= currentLives) active = false; // if lives reported is 0, then diff
            livesCount[i].enabled = active;
        }
    }

    // Update the game level here
    protected void updateGameLevel(int score, int time = 0) {
        gameLevel++;
        targetText.text = "Targets: " + gameLevel.ToString("D2");
    }

    protected void resetUIStats() {
        currentScore = 0;
        gameLevel = 0;
        targetText.text = "Targets: " + gameLevel.ToString("D2");
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

    // Add time here when ball hits a target
    protected void addTime(int timeToAdd) {
        float timeInMins, timeInSecs;
        TimeSpan convertedTime; // .Net class for changing time given an integer

        // first, add the time to the current timer;
        gameTimerSecs += timeToAdd;

        // then, convert the timer to both minutes and seconds using FromSeconds
        convertedTime = TimeSpan.FromSeconds(gameTimerSecs);
        
        // then, apply the converted time into the minutes and seconds field
        timers[1] = convertedTime.Seconds;
        timers[2] = convertedTime.Minutes;

        // then, update the timer text
        updateTimerTexts(1, timers[1].ToString("D2"));
        updateTimerTexts(2, timers[2].ToString("D2"));
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
            while (pauseGame) yield return null;
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
