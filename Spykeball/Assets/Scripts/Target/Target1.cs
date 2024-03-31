using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Target that moves around from side to side
public class Target1 : Target
{
    public float minSpeed, maxSpeed, duration;
    protected float moveTimer, moveTimerRem;

    protected int directionX = 1;

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
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

    
    protected override void doOnTimersSaved(bool state) {
        if (state) moveTimerRem = moveTimer - Time.time; // get the difference between the moveTimer's currentTime
        else moveTimer = Time.time + moveTimerRem; // apply the difference between the moveTimer's currentTime
    }
}
