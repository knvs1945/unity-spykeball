using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Target that moves to one side straight only
public class Target2 : Target
{
     public float minSpeed, maxSpeed;
    protected float moveTimer;
    protected int directionX = 1;

    // Update is called once per frame
    void Update()
    {
        moveSideways();
    }

    protected void moveSideways() {
        transform.Translate(Vector2.right * directionX * moveSpeed * Time.deltaTime, Space.World);
    }

    protected override void doOnApplyLevel() {
        moveSpeed = Random.Range(minSpeed, maxSpeed) * Mathf.Min(finalSpeed, Mathf.Max(1, (Level / 10f)));
        directionX = (int) Mathf.Sign(Random.Range(-1f,1f));
    }
}
