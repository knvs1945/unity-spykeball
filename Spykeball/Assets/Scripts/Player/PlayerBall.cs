using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : GameUnit
{

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

    protected void OnCollisionEnter2D(Collision2D collision) {
        if (rb.velocity.y > 3) anim.SetTrigger("bounce"); // only bounce the ball if its going faster than 1;

    }
}
