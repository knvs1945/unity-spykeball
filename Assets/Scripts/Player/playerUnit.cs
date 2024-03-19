using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerUnit class - for managing the player object directly manipulated by the player. 
/// Also holds the buffs and skills manager inherited by player objects
/// </summary>

public class PlayerUnit : GameUnit
{    
    
    // Managers
    protected BuffManager buffList = new BuffManager();
    protected SkillManager skillList;

    protected PlayerControls controls;
    protected bool isControlDisabled = true, isdamageShldActive = false;
    protected buffStatsList playerBuffs;

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

    // common functions
    protected bool checkHPifAlive() {
        if (isAlive && !isNPC) {
            return isAlive;
        }
        return false;
    }

    protected void updateHPBar() {
        consoleUI.Log("Updating HP Bar: " + HP);
        PlayerUnit.updatePlayerHPBar();    // inform the playerHandler that the HP bar needs updating
    }

    // overridden methods from gameUnit. factor should be -1 if removing a buff
    public override void addBuff(float amount, string statname, Buff buffToAdd, int factor = 1, bool isRemoveBuff = false) {
        // no use for buffs if it's less than zero;
        if (amount <= 0) return;

        amount *= factor;
        switch (statname) {
            case "hp":
            case "HP":  playerBuffs.HPmin += amount;
                        break;
            case "movespeed":
            case "MOVESPEED":   playerBuffs.moveSpeed += amount;
                                break;
            case "atkmin":
            case "ATKMIN": playerBuffs.ATKmin += amount;
                            break;
        }
        if (!isRemoveBuff) {
            Debug.Log("Add Buff Started: " + amount);
            buffList.addBuffToList(buffToAdd);
        }
    }

}

// class for keeping buff stats for easier management
public class buffStatsList{
    public float HPmin, moveSpeed, ATKmin;

    // constructor here
    public buffStatsList (float hpmin = 0, float movespeed = 0, float atkmin = 0) {
        HPmin = hpmin;
        moveSpeed = movespeed;
        ATKmin = ATKmin;
    }

    // force resetStats here;
    public void resetStats () {
        HPmin = 0;
        moveSpeed = 0;
        ATKmin = 0;
    }
}
