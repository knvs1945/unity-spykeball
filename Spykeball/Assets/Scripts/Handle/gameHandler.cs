using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Handler
{
    
    [SerializeField]
    protected PlayerHandler playerHandle;
    [SerializeField]
    protected RhythmHandler rhythmHandle;
    [SerializeField]
    protected EnemyHandler enemyHandle;
    [SerializeField]
    protected GoalHandler goalHandle;
    [SerializeField]
    protected MapHandler mapHandle;
    private bool stagePrepFlag;


    // Start is called before the first frame update

    void Start()
    {
        // fix framerate to default FPS (30);
        Application.targetFrameRate = FPS;

        // test setBPM
        // RhythmHandler.setBPM(60);
        consoleUI.Log("State: " + GameHandler.getGameState());

        // move this somewhere else once startLevel works
        startLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    // get gameState
    public static states getGameState() {
        return gameState;
    }

    public static bool isGamePaused() {
        return pauseGame;
    }

    /* 
    *
    *   Start the stage sequences
    *
    */
    public bool startLevel() {
        stagePrepFlag = false;
        if (gameState != states.inStage) {
            Debug.Log("GameHandler: starting level...");
           

                // only change the gameState if the stage preps completes
                StartCoroutine(startStagePreps(result => {
                    if (result) {
                        Debug.Log("Start Stage Preparations completed");
                        gameState = states.inStage;
                    }
                    else Debug.Log ("An error occurred during stage preparation.");
                }));
            }
        return stagePrepFlag;
    }

    // prep sequences
    IEnumerator startStagePreps(Action<bool> checkPrepDone) {
        yield return StartCoroutine(mapGoalsStagePrep());
        
        checkPrepDone(stagePrepFlag); // all coroutines returned properly so we return true
    }

    IEnumerator mapGoalsStagePrep() {
        bool success = false;
        string defaultGoalTitle = "Destroy Obelisks";
        string defaultGoalDesc = "Destroy the obelisk block the next area";
        Goal thisGoal, prevGoal = null;

        List<Gatekeeper> gkList;

         // 1. get the map handler's Gatekeeper spawns and pass it to the Enemy Handler
        if (enemyHandle.spawnGateKeepers(mapHandle.GatekeeperSpawns, gameStage)) {
                // 2. Create a goals per spawned gatekeeper and assign MapHandle as its owner
                mapHandle.CurrentGKs = enemyHandle.Gatekeepers;
                for (int i = 0; i < mapHandle.CurrentGKs.Count; i++) {
                    Debug.Log ("Checking Goal Handler: " + goalHandle);
                    thisGoal = goalHandle.CreateGoal(
                        Goal.TYPE_KILL, 
                        defaultGoalTitle, 
                        defaultGoalDesc, 
                        mapHandle.CurrentGKs[i], true, 
                        0, 0, 
                        null, 
                        prevGoal
                    );
                    // we only need to create Gatekeepers - 1 pathblockers since the last GK will initiate the boss fight
                    if (i < mapHandle.CurrentGKs.Count - 1) mapHandle.spawnPathblockers(mapHandle.CurrentGKs[i], i); // spawn pathblocker per Gatekeeper here;
                    if (thisGoal != null) thisGoal.Owner = mapHandle;
                    if (prevGoal == null) prevGoal = thisGoal;
                }

            yield return StartCoroutine(enemyStagePrep());
            success = true;
        }
        yield return success;

    }

    IEnumerator enemyStagePrep() {
        bool success = false;
        Enemy tempEnemy;
        int monsterCount = 5;
        float distanceFromGK = 3f;
        // 3. Create monsters per gatekeeper available
        for (int i = 0; i <mapHandle.CurrentGKs.Count; i++) {
            for (int j = 0; j < monsterCount; j++) {
                
                // Debug.Log("MapHandle: Spawning monster for GK-" + i + ": " + j);
                tempEnemy = enemyHandle.spawnEnemy(mapHandle.CurrentGKs[i], distanceFromGK, distanceFromGK, 0); // let's set areaID to default 0 for now;
                if (tempEnemy != null) mapHandle.CurrentGKs[i].addAsDefender(tempEnemy);
            }
        }
        success = true;
        yield return StartCoroutine(playerStagePrep());
        yield return success;
    }
    
    IEnumerator playerStagePrep() {
        bool success = false;
        if (playerHandle.startStageSequence()) {
            // call rhythm stage prep next
            yield return StartCoroutine(rhythmStagePrep());
            success = true;
        }
        else stagePrepFlag = true;
        yield return success;
    }

    IEnumerator rhythmStagePrep() {
        
        yield return new WaitForSeconds(stageIntroTimer + 1);
        bool success = false;
        goalHandle.IsCheckingGoals = true;
        rhythmHandle.playMetronome();
        rhythmHandle.playTrack(1);
        // enemyHandle.startGenerator(0,1); // start spawning monsters    
        // Enemy.isEnemyEnabled = true; // true = unpauses the enemy
        success = true;
        // yield return success;
    }


    
}
