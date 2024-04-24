using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Floating Target Classes
public class Target : GameUnit
{
    protected const float finalSpeed = 25f;

    // Delegates and Events
    public delegate void onDestroyTarget();
    public static event onDestroyTarget doOnDestroyTarget;

    // explosion effect
    public ParticleSystem targetBreak;
    
    protected Rigidbody2D rb;
    protected Animator anim;
    protected bool isBroken = false;

    // inherited timer flags
    protected bool timersSaved = false; // this will inform the class that the timer data have already been saved during a pause

    public bool IsBroken {
        get { return isBroken; }
        private set { isBroken = value; }
    }

    void Start()
    {
        gameObject.GetComponent<Animator>().Play("target_intro");
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == "Ball") {
            destroyTarget();
        }
    }

    // destroy and respawn new target. Set noSpawn to true if the targets don't need to spawn
    protected void destroyTarget(bool noRespawn = false) {
        if (!noRespawn) {
            Instantiate(targetBreak, transform.position, Quaternion.identity);
            PlayerBall.lastTargetPos = transform.position;
            doOnDestroyTarget(); // inform listeners that a target has broken
        }
        doOnBreak();
    }
    
    // restart target spawn by destroying the current one
    public void restartTarget() {
        destroyTarget(true);
    }

    public void applyLevel(int level = 1) {
        Level = level;
        timersSaved = false; // default timersSaved as false
        doOnApplyLevel();
    }

    // overrideable apply level method
    protected virtual void doOnApplyLevel() {}

    // pause player components to check if it is paused or not
    protected bool checkIfGamePaused() {
        
        if (isGamePaused) {
            if (rb.simulated) rb.simulated = false; // disable rigidbody physics when paused
            if (anim.speed != 0) anim.speed = 0;    // disable animator when paused
        }
        else {
            if (!rb.simulated) rb.simulated = true;
            if (anim.speed == 0) anim.speed = 1;
        }
        pauseTimers(isGamePaused);

        return isGamePaused;
    }

    protected void pauseTimers(bool state) {
        if (state) {
            if (!timersSaved) {
                timersSaved = true;
                doOnTimersSaved(state);
            }
        }
        else {
            if (timersSaved) {
                timersSaved = false;
                doOnTimersSaved(state);
            }
        }
    }

    protected virtual void doOnTimersSaved(bool state) {}
    protected void doOnBreak() {
        isBroken = true;
        Destroy(gameObject);
    }

}
