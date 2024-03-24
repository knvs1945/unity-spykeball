using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

    // delegates and events
    public delegate void onHitTarget(int score);
    public event onHitTarget doOnHitTarget;

    public int baseScore;

    private Animator anim;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected void Awake()
    {
        anim = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
