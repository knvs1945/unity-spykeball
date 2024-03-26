using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

    protected const float startPosX = 0, startPosY = 4;

    // delegates and events
    public delegate void onHitTarget(int score);
    public event onHitTarget doOnHitTarget;

    public int baseScore;
    public float boundsFloor, boundsCeiling, boundsLeft, boundsRight;

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
    }


    // if the ball hits 
    protected void OnCollisionEnter2D(Collision2D collision) {
        int scoreToAdd;
        if (collision.collider.tag == "Target") {
            scoreToAdd = baseScore + ((int)Mathf.Abs(rb.velocity.y) * 5);
            doOnHitTarget( scoreToAdd ); // use the current speed as the score
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
