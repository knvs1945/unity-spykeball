using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Target that moves around from side to side
public class Target1 : Target
{
    public float minSpeed, maxSpeed, duration;
    protected float moveTimer;

    protected int directionX = 1;

    // Update is called once per frame
    void Update()
    {
        moveSideways();
    }

    protected void moveSideways() {
        if (moveTimer > Time.time) {
            transform.Translate(Vector2.right * directionX * moveSpeed * Time.deltaTime, Space.World);
        }
        else {
            moveTimer = Time.time + duration;
            directionX *= -1;
        }
    }

    protected override void doOnApplyLevel() {
        moveSpeed = Random.Range(minSpeed, maxSpeed) * Mathf.Min(finalSpeed, Mathf.Max(1, (Level / 10f)));
    }
}
