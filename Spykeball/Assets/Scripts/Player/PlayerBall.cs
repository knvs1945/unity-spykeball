using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

    protected const float startPosX = 0, startPosY = 4, MAX_velocity = 17.5f;
    protected const int COUNT_Lives = 4, TIME_addTime = 3;
    protected const int MODE_survival = 1, MODE_timeattack = 2;
    

    // delegates and events
    public delegate void onHitTarget(int score, int timeToAdd = 0);
    public delegate void onLivesLeft(int livesleft);
    public delegate void onNoMoreLives();

    public event onHitTarget doOnHitTarget;
    public event onNoMoreLives doOnNoMoreLives;
    public event onLivesLeft doOnLivesLeft;
    public int baseScore;
    public float boundsFloor, boundsCeiling, boundsLeft, boundsRight;

    protected int lives, mode;
    protected Color baseColor = new Color(1,1,1,1);

    private Animator anim;
    private Rigidbody2D rb;
    private Renderer rbRender;

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
    }

    // Update is called once per frame
    void Update()
    {
        checkIfOutOfBounds();
    }

    // Fixed Update - called for physics-based behavior
    void FixedUpdate()
    {
        if (mode == MODE_timeattack) {
            checkBallVelocity();
        }
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

        transform.position = new Vector2(startPosX, startPosY);
        rbRender.material.color = baseColor;
        rb.velocity = new Vector2(0,0);
    }

    // if the ball hits 
    protected void OnCollisionEnter2D(Collision2D collision) {
        int scoreToAdd, timeToAdd = 0;
        if (collision.collider.tag == "Target") {
            scoreToAdd = baseScore + ((int)Mathf.Abs(rb.velocity.y) * 5);

            if (mode == MODE_survival) {
                rbRender.material.color = baseColor; // reset the color back to max lives    
                lives = COUNT_Lives; // restore the ball's lives back to maximum;
                doOnLivesLeft(lives);
            }
            else if (mode == MODE_timeattack) {
                timeToAdd = TIME_addTime + ((int) Mathf.Abs(rb.velocity.magnitude / 3)); // add maximum of 6 seconds based on ball's speed capped at 6 seconds
            }

            doOnHitTarget(scoreToAdd, timeToAdd); // report the score and time to add
        }
        if (collision.collider.tag == "Floor") {
            if (mode == MODE_survival) deductLives();
        }
        if (rb.velocity.y > 3) anim.SetTrigger("bounce"); // only bounce the ball if its going faster than 1;
    }

    // reduce the lives here
    protected void deductLives() {
        float currentColor = 1, colorfactor = 0.25f;

        lives--;
        Debug.Log("Lives Left: " + lives);
        currentColor = currentColor - (colorfactor * (COUNT_Lives - lives));
        rbRender.material.color = new Color(1, currentColor, currentColor, 1); // color starts getting redder per bounce
        doOnLivesLeft(lives);
        if (lives <= 0) doOnNoMoreLives();
    }

    // move the ball back within game bounds
    protected void checkIfOutOfBounds() {
        if (transform.position.y >= 10f) transform.position = new Vector2(transform.position.x, boundsFloor);
        if (transform.position.y <= -10f) transform.position = new Vector2(transform.position.x, boundsCeiling);
        if (transform.position.x <= -12f) transform.position = new Vector2(boundsRight, transform.position.y);
        if (transform.position.x >= 12f) transform.position = new Vector2(boundsRight, transform.position.y);
    }

    // Cap the ball's maximum velocity if it is on time attack 
    protected void checkBallVelocity() {
        
        if (rb.velocity.magnitude > MAX_velocity) {
            Debug.Log("Ball exceeding max velocity: " + rb.velocity.magnitude);
            rb.velocity = rb.velocity.normalized * MAX_velocity; // cap the velocity to the max velocity on time attack
        }

    }
}
