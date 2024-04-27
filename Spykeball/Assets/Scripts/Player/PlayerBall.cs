using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

    protected const float MAX_velocity = 17.5f, MAX_velocity_unli = 20f, MIN_effectSpd = 13f, effectsGap = 0.05f;
    protected const int COUNT_Lives = 4, TIME_addTime = 3;
    protected const int MODE_survival = 1, MODE_timeattack = 2;
    
    public static Vector2 lastTargetPos;

    // delegates and events
    public delegate void onHitTarget(int score, int timeToAdd = 0);
    public delegate void onLivesLeft(int livesleft);
    public delegate void onNoMoreLives();
    public event onHitTarget doOnHitTarget;
    public event onHitTarget doOnHitFloor;
    public event onNoMoreLives doOnNoMoreLives;
    public event onLivesLeft doOnLivesLeft;

    // hashset is used to store a gameobject that the ball already collided with. This is used for floors
    private Sprite baseSprite;
    
    public Vector2 startPosition;
    public int baseScore;
    public float boundsFloor, boundsCeiling, boundsLeft, boundsRight;
    
    protected Color baseColor = new Color(1,1,1,1);
    protected int lives, mode;
    protected float effectTimer;
    protected bool isHighVelocity = false, hasHitAFloor = false;

    private Animator anim;
    private Rigidbody2D rb;
    private Renderer rbRender;
    private SpriteRenderer spriteRnd;

    // Start is called before the first frame update
    void Start()
    {
        restartUnit("normal");
    }

    protected void Awake()
    {
        anim = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>();
        rbRender = GetComponent<Renderer>();
        spriteRnd = GetComponent<SpriteRenderer>();
        baseSprite = spriteRnd.sprite;
        registerEvents();
    }

    // register listeners
    protected void registerEvents() {
        Target.doOnDestroyTarget += addScoreOrTime;
    }

    // prevent the ball from getting stuck in a certain animation frame
    protected void OnEnable() {
        if (anim != null) {
            Debug.Log("Resetting player ball");
            anim.Update(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
        checkIfOutOfBounds();
    }

    // Fixed Update - called for physics-based behavior
    void FixedUpdate()
    {   
        checkBallVelocity();
        createSpdEffects();
    }

    // force the animation to get back to "ball_idle" state before getting disabled
    public void deactivate() {
        anim.Play("ball_idle");
        spriteRnd.sprite = baseSprite;
        gameObject.SetActive(false);
    }

    public override void restartUnit(string gameMode) {    
        switch(gameMode) {
            case "Survival":
                mode = MODE_survival;
                lives = COUNT_Lives;
                break;
            case "Time Attack":
                mode = MODE_timeattack;
                break;
        }

        transform.position = startPosition;
        rbRender.material.color = baseColor;
        rb.velocity = new Vector2(0,0);
    }

    // if the ball hits something
    protected void OnCollisionEnter2D(Collision2D collision) {
        int scoreToAdd = 0, timeToAdd = 0;
        Target targetHit;
        if (!hasHitAFloor && collision.collider.tag == "Floor") {
            hasHitAFloor = true;
            if (mode == MODE_survival) deductLives();
            // deduct one second every time the ball hits the floor
            if (mode == MODE_timeattack) {
                timeToAdd = -1;
                doOnHitFloor(scoreToAdd, timeToAdd); // report the score and time to add
                EffectHandler.Instance.CreateEffectScoreText(transform.position, "" + timeToAdd + " sec", true);
            }
            SoundHandler.Instance.playSFX(SFXType.Bounce); // play bouncing sound effect
        }
        if (rb.velocity.y > 3) anim.SetTrigger("bounce"); // only bounce the ball if its going faster than 1;
    }

    // if the ball exits a collision from something
    protected void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Floor")) {
            hasHitAFloor = false;
        }
    }

    // listens to target doOnDestroy event to add score, life or time depending on game mode
    protected void addScoreOrTime() {
        int scoreToAdd = 0, timeToAdd = 0;
        scoreToAdd = baseScore + ((int)Mathf.Abs(rb.velocity.y) * 5);
        
        if (mode == MODE_survival) {
            rbRender.material.color = baseColor; // reset the color back to max lives    
            lives = COUNT_Lives; // restore the ball's lives back to maximum;
            doOnLivesLeft(lives);
            // show added score 
            EffectHandler.Instance.CreateEffectScoreText(lastTargetPos, "+" + scoreToAdd);
        }
        else if (mode == MODE_timeattack) {
            // add maximum of 6 seconds based on ball's speed capped at 6 seconds
            timeToAdd = TIME_addTime + ((int) Mathf.Abs(rb.velocity.magnitude / 3)); 
            // show added time
            EffectHandler.Instance.CreateEffectScoreText(lastTargetPos, "+" + timeToAdd + " secs");
        }

        doOnHitTarget(scoreToAdd, timeToAdd); // report the score and time to add
    }

    // reduce the lives here
    protected void deductLives() {
        float currentColor = 1, colorfactor = 0.25f;

        lives--;
        Debug.Log("Lives Left: " + lives);
        currentColor = currentColor - (colorfactor * (COUNT_Lives - lives));
        rbRender.material.color = new Color(1, currentColor, currentColor, 1); // color starts getting redder per bounce
        doOnLivesLeft(lives);
        if (lives <= 0) {
            SoundHandler.Instance.playGameTrack(1, false); // play gameend sound
            doOnNoMoreLives(); // fire noMoreLives event
        }
    }

    // move the ball back within game bounds
    protected void checkIfOutOfBounds() {
        if (transform.position.y >= 15f) transform.position = new Vector2(transform.position.x, boundsFloor);
        if (transform.position.y <= -15f) transform.position = new Vector2(transform.position.x, boundsCeiling);
        if (transform.position.x <= -12f) transform.position = new Vector2(boundsRight, transform.position.y);
        if (transform.position.x >= 12f) transform.position = new Vector2(boundsRight, transform.position.y);
    }

    // pause ball components and check if it is paused or not
    protected bool checkIfGamePaused() {
        
        if (isGamePaused) {
            if (rb.simulated) rb.simulated = false; // disable rigidbody physics when paused
            if (anim.speed != 0) anim.speed = 0;    // disable animator when paused
        }
        else {
            if (!rb.simulated) rb.simulated = true;
            if (anim.speed == 0) anim.speed = 1;
        }

        return isGamePaused;
    }

    // Cap the ball's maximum velocity if it is on time attack 
    protected void checkBallVelocity() {
        
        if (mode == MODE_survival) {
            if (rb.velocity.magnitude > MAX_velocity_unli) {
                rb.velocity = rb.velocity.normalized * MAX_velocity_unli; // cap the velocity to the max velocity on time attack
            }
        }
        
        else if (mode == MODE_timeattack) {
            if (rb.velocity.magnitude > MAX_velocity) {
                rb.velocity = rb.velocity.normalized * MAX_velocity; // cap the velocity to the max velocity on time attack
            }
        }

        // check if the velocity DID pass at least the MIN_effectSpd before creating mirages
        if (rb.velocity.magnitude > MIN_effectSpd) {
            if (!isHighVelocity) {
                isHighVelocity = true;
                effectTimer = Time.time + effectsGap;
            }
        }
        else {
            if (isHighVelocity) isHighVelocity = false;
        }
    }

    // create effects only when ball is super fast
    protected void createSpdEffects() {
        if (!isHighVelocity) return;
        if (Time.time < effectTimer) return;

        EffectHandler.Instance.CreateEffectSpeedMirage(transform.position, spriteRnd.sprite, false, spriteRnd.material.color);
        effectTimer = Time.time + effectsGap; // reload the timer
    }
}
