using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

    protected const float startPosX = 0, startPosY = 4;
    protected const int COUNT_Lives = 3;

    // delegates and events
    public delegate void onHitTarget(int score);
    public delegate void onNoMoreLives();

    public event onHitTarget doOnHitTarget;
    public event onNoMoreLives doOnNoMoreLives;
    public int baseScore;
    public float boundsFloor, boundsCeiling, boundsLeft, boundsRight;

    protected int lives;
    private Animator anim;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        restartUnit("normal");
    }

    protected void Awake()
    {
        anim = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        checkIfOutOfBounds();
    }

    public override void restartUnit(string gameMode) {    
        transform.position = new Vector2(startPosX, startPosY);
        rb.velocity = new Vector2(0,0);
        lives = COUNT_Lives;
    }


    // if the ball hits 
    protected void OnCollisionEnter2D(Collision2D collision) {
        int scoreToAdd;
        if (collision.collider.tag == "Target") {
            scoreToAdd = baseScore + ((int)Mathf.Abs(rb.velocity.y) * 5);
            doOnHitTarget( scoreToAdd ); // use the current speed as the score
            lives = COUNT_Lives; // restore the ball's lives back to maximum;
        }
        if (collision.collider.tag == "Floor") {
            lives--;
            Debug.Log("Lives Left: " + lives);
            if (lives <= 0) doOnNoMoreLives();
        }
        if (rb.velocity.y > 3) anim.SetTrigger("bounce"); // only bounce the ball if its going faster than 1;
        
    }

    // move the ball back within game bounds
    protected void checkIfOutOfBounds() {
        if (transform.position.y >= 10f) transform.position = new Vector2(transform.position.x, boundsFloor);
        if (transform.position.y <= -10f) transform.position = new Vector2(transform.position.x, boundsCeiling);
        if (transform.position.x <= -12f) transform.position = new Vector2(boundsRight, transform.position.y);
        if (transform.position.x >= 12f) transform.position = new Vector2(boundsRight, transform.position.y);
    }
}
