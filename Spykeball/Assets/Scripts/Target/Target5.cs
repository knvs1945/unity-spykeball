using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dodges the ball for N amount of times
public class Target5 : Target
{
    public float minX, maxX, minY, maxY;
    public int maxDodges;

    protected Vector2 nextPos;
    protected float moveTimer, currentDodges;
    protected bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        moveToNextPos();
    }

    protected void moveToNextPos() {
        if (isMoving) {
            if (Vector2.Distance(transform.position, nextPos) > 0.1f) {
                transform.position = Vector2.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                Debug.Log("Now Moving from: " + transform.position + " towards: " + nextPos + " at speed: " + moveSpeed);
            }
            else isMoving = false;
        }
    }

    // only trigger destruction if run out of dodges
    protected override void OnCollisionEnter2D (Collision2D collision) {
        if (isMoving) return;
        if (collision.collider.tag == "Ball") {
            if (currentDodges > 0) {
                currentDodges--;
                Debug.Log("Dodges Left: " + currentDodges);
                generateNextPos();
                isMoving = true;
            }
            else destroyTarget();
        }
    }

    // generate new position to move to
    protected void generateNextPos() {
        float xpos = Random.Range(minX, maxX);
        float ypos = Random.Range(minY, maxY);
        nextPos = new Vector2(xpos, ypos);
    }

    protected override void doOnApplyLevel() {
        currentDodges = maxDodges + (int) Mathf.Min(5, Level/10);
        Debug.Log("Dodges Set: " + currentDodges);
        moveSpeed = 20f;
        generateNextPos();
    }
}
