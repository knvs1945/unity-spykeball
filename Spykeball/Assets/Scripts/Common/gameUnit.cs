using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour
{
    public float HP, moveSpeed;

    protected int Level = 1;
    protected float HPMax, HPTemp;
    protected float ATKbase, ATKmax, ATKTemp, ATKdelay, ATKTimer, ATKRange;
    protected bool isAlive = true, isActive, isPaused = false, isImmune = false, isNPC = false, isIllusion;
    
    [SerializeField]
    protected string unitName;

    // constructor
    public GameUnit() {
    }

    // getters
    public string Name {
        get { return unitName; }
    }
    public bool IsAlive {
        get { return isAlive; }
    }

    public bool IsActive {
        get { return isActive; }
    }

    public bool IsPaused {
        get { return isPaused; }
    }

    public bool IsImmune {
        get { return isImmune; }
    }

    public bool IsNPC {
        get { return isNPC; }
    }

    public bool IsIllusion {
        get { return isIllusion; }
    }

    // common unit behavior
    public bool takesDamage(float DMG) {
        Debug.Log("Target takes damage! " + this);
        if (!isImmune) {
            doOnTakeDamage(DMG);
            return true;
        }
        return false;
    }

    // overrideable add buff and removeBuff methods for game units
    public virtual void addBuff(int amount, string statname, Buff buffToAdd, int factor = 1, bool isRemoveBuff = false) { }
    public virtual void addBuff(float amount, string statname, Buff buffToAdd, int factor = 1, bool isRemoveBuff = false) { }

    // force kill a unit
    public bool killUnit() {
        if (isAlive) {
            doOnDeath();
            return true;
        }
        return false;
    }

    // Overrideables
    protected virtual void doOnTakeDamage(float DMG) {}
    protected virtual void doOnDeath() {}

}
