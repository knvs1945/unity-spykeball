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
    public BallMarker ballMarker;
    public GameObject panelMainMenu, panelRestartMenu, panelRoundPanel;
    public GameObject panelTimer, panelLives, panelScore, panelTargets, panelPauseMenu, panelControls; // ingame panels
    public Image[] livesCount;
    public Text[] timerText;
    public int playerLives;
    

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

        // Stop the previous timer if it was ever started
        stopTimer();

        // enable needed panels and set up the UI
        if (Mode == Modes.Survival) {
            Panel.gameMode = "Survival";
            panelScore.SetActive(true);
            panelLives.SetActive(true);
            panelTimer.SetActive(false);
            updateLives(playerLives);
        }
        else if (Mode == Modes.TimeAttack) {
            Panel.gameMode = "Time Attack";
            panelScore.SetActive(true);
            panelLives.SetActive(false);
            panelTimer.SetActive(true);
            setTimerTexts();
        }

        panelTargets.SetActive(true);
        panelMainMenu.SetActive(false);
        panelRestartMenu.SetActive(false);
        panelPauseMenu.SetActive(false);
        panelControls.SetActive(false);
        panelRoundPanel.SetActive(true);
        playPanelIntro(panelRoundPanel);
        resetUIStats();   

        // game is starting, set the game state as needed
        gameState = states.inStage;
    }

    // restarts the attack timer after the intro
    public void restartTimeAttackTimer() {
        startTimer();
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
        // stop the timer if it was originally playing
        stopTimer();

        // hide the ingame panels
        panelScore.SetActive(false);
        panelLives.SetActive(false);
        panelTimer.SetActive(false);
        panelTargets.SetActive(false);
        panelRoundPanel.SetActive(false);

        panelMainMenu.SetActive(true);
        panelRestartMenu.SetActive(false);
        panelPauseMenu.SetActive(false);
        panelControls.SetActive(false);

        playPanelIntro(panelMainMenu);
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
            SoundHandler.Instance.setVolumeValues(panelControls.GetComponent<ControlPanel>().getCurrentVolumeUI());
            openSettingsPanel(false); // close the control panel;
        }
    }

    // set the controls from the player to display in the control panel
    public void setCurrentControls(PlayerControls controls) {
        panelControls.GetComponent<ControlPanel>().setCurrentControlUI(controls);
        Panel.controls = controls;
    }

    public void setCurrentVolumes(float[] values) {
        panelControls.GetComponent<ControlPanel>().setCurrentVolumeUI(values);
    }

    public PlayerControls getNewControls() {
        PlayerControls newControl = panelControls.GetComponent<ControlPanel>().getCurrentControl();
        Panel.controls = newControl;
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

    ///
    /// Play animations here
    ///

    protected void playPanelIntro(GameObject panel) {
        Panel temp = panel.GetComponent<Panel>();
        if (temp != null) {
            temp.playPanelIntro();
        }
    }

    /// <summary>
    /// 
    /// Timer functions here
    /// 
    /// </summary>

    // Set timer texts
    protected void setTimerTexts() {
        
        // refresh the timer & timer counters
        timers[0] = 0;
        timers[1] = 0;
        timers[2] = 0;
        gameTimerSecs = 0;

        timerText[0].text = "00"; // msecs
        timerText[1].text = "00"; // secs
        timerText[2].text = "00"; // mins

        addTime((int) timeAttackLimit);
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
        StartCoroutine("countDownSecs");
        StartCoroutine("countDownMsecs"); // countdown milliseconds for pure aesthetic
    }

    protected void stopTimer() {
        StopCoroutine("countDownSecs");
        StopCoroutine("countDownMsecs"); // countdown milliseconds for pure aesthetic
    }

    // countDown milliseconds
    IEnumerator countDownSecs() {
        while (gameTimerSecs > 0) {        
            while (pauseGame) yield return null;
            timers[1]--; 
            gameTimerSecs--;               
            if (timers[1] < 0) {
                timers[1] = 59;
                timers[2]--; // update the minutes
                if (timers[2] < 0) {
                    timers[2] = 59;         // end game here
                }
                updateTimerTexts(2, timers[2].ToString("D2"));
            }
            updateTimerTexts(1, timers[1].ToString("D2"));

            // start displaying time alerts
            if (gameTimerSecs <= 10){
                Debug.Log("Starting Timer Text");
                EffectHandler.Instance.CreateEffectAlertText(new Vector2(0,0), gameTimerSecs.ToString());
            }
            yield return new WaitForSeconds(1f); // return every 10 msecs
        }
        
        if (gameTimerSecs <= 0) {
            stopTimer();
            timers[1] = 0;
            timers[2] = 0;
            updateTimerTexts(1, timers[1].ToString("D2"));
            updateTimerTexts(2, timers[2].ToString("D2"));
            Debug.Log("Timer has ended");
            SoundHandler.Instance.playGameTrack(1, false); // play gameend sound
            doOnTimeRunOut();
        }
        yield return true;
    }

    // countDown milliseconds. Usedpurely for aesthetic
    IEnumerator countDownMsecs() {
        while (gameTimerSecs >= 0) {        
            while (pauseGame) yield return null;
            timers[0]--; 
            if (timers[0] < 0) timers[0] = 9;
            updateTimerTexts(0, timers[0].ToString("D2"));
            yield return new WaitForSeconds(0.1f); // return every 10 msecs
        }
        
        if (gameTimerSecs <= 0) {
            timers[0] = 0;
            updateTimerTexts(0, timers[0].ToString("D2")); // fix msces display to 0
            yield return true;
        }
    }

}
