using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Target that moves to one side straight but alternates up and down 
public class Target3 : Target
{
    public float minSpeed, maxSpeed;
    protected float moveTimer, moveTimerRem, vertSpeed = 0f, duration = 1f;
    protected int directionX = 1, directionY;

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
        moveSideways();
    }

    protected void moveSideways() {
        transform.Translate(Vector2.right * directionX * moveSpeed * Time.deltaTime, Space.World);
        if (moveTimer > Time.time) transform.Translate(Vector2.up * directionY * vertSpeed * Time.deltaTime, Space.World);
        else {
            directionY *= -1;
            moveTimer = Time.time + duration;
        }
    }

    protected override void doOnApplyLevel() {
        moveSpeed = Random.Range(minSpeed, maxSpeed) * Mathf.Min(finalSpeed, Mathf.Max(1, (Level / 10f)));
        vertSpeed = Random.Range(minSpeed, maxSpeed);
        directionX = (int) Mathf.Sign(Random.Range(-1f,1f));
        directionY = (int) Mathf.Sign(Random.Range(-1f,1f));
        moveTimer = Time.time + duration;

        Debug.Log("Target 3: " + vertSpeed + " - " + directionY);
    }
    
    protected override void doOnTimersSaved(bool state) {
        if (state) moveTimerRem = moveTimer - Time.time; // get the difference between the moveTimer's currentTime
        else moveTimer = Time.time + moveTimerRem; // apply the difference between the moveTimer's currentTime
    }
}

