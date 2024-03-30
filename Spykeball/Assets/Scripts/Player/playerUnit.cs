using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerUnit class - for managing the player object directly manipulated by the player. 
/// Also holds the buffs and skills manager inherited by player objects
/// </summary>

public class PlayerUnit : GameUnit
{    
    // events and delegates
    public delegate void onPausePressed(bool state);
    public static event onPausePressed doOnPausePressed;
    
    // Managers
    protected PlayerControls controls;
    protected bool isControlDisabled = true, isdamageShldActive = false;
    
    public float base_DMGdelay; // damage shield between damage instances

    [SerializeField]
    private PlayerUnit player;

    // Event Delegates here
    public delegate void onPlayerTakesDamage(); 

    // Events start here
    public static event onPlayerTakesDamage updatePlayerHPBar; 

    // Start is called before the first frame update
    protected virtual void Start()
    {
        isImmune = false;
        Debug.Log("Player check: " + player);
    }

    public PlayerControls Controls {
        get { return controls; }
        set { 
            controls = value; 
            if (player != null) player.Controls = controls;    
        }
    }

    public PlayerUnit Player {
        get { return player;  }
        set { player = value; }
    }

    public bool IsControlDisabled {
        get { return isControlDisabled; }
        set { isControlDisabled = value; }
    }

    public bool IsDamageShldActive {
        get { return isdamageShldActive;}
    }

    // restart player unit    
    public override void restartUnit(string gameMode) {
        Debug.Log("Restarting player unit");
        player.restartUnit(gameMode);
    }

    // common functions
    protected bool checkHPifAlive() {
        if (isAlive && !isNPC) {
            return isAlive;
        }
        return false;
    }

    protected void updateHPBar() {
        PlayerUnit.updatePlayerHPBar();    // inform the playerHandler that the HP bar needs updating
    }
    
    protected void playerPressedPause(bool pauseEvent) {
        Debug.Log("Pausing game... " + pauseEvent);
        doOnPausePressed?.Invoke(pauseEvent);
    }

    // overridden methods from gameUnit. factor should be -1 if removing a buff

}
