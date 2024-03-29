using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game States
public enum states
{
    MainMenu,
    inStage,
    inEliteBattle,
    inBossBattle,
    inMap,
    Victory,
    Defeat,
    GameWin,
    GameLoss,
    GameEnd,
    Cutscene,
    Preload
}

// Game Modes
public enum Modes
{
    Survival,
    TimeAttack
}

public class Handler : MonoBehaviour
{
    // delegates and events
    public delegate void onGameOver();
    public static onGameOver doOnGameOver;

    protected static states gameState = states.Preload; // default the gameState to preload
    protected static Modes Mode = Modes.Survival; // default the gameMode to Survival

    protected static bool pauseGame = false;
    protected static int FPS = 30;
    protected static float stageIntroTimer = 3f;

    // game-shared data
    protected static int gameStage = 1;

    // A goal will inform its handler that it has been completed
    // public virtual void goalCompleted (Goal goal) { }
    // public virtual void goalFailed (Goal goal) { }

    public void restartHandler(string gameMode) {
        doOnRestartHandler(gameMode);
    }

    protected virtual void doOnRestartHandler(string gameMode) {}
}
