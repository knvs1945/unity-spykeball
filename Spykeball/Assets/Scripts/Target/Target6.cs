using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dodges the ball indeterminately unless hit within a given time limit
public class Target6 : Target
{
    public float minX, maxX, minY, maxY, maxDuration;

    protected Renderer rbRender;
    protected Vector2 nextPos;
    protected Color baseColor = new Color(0,0,0,1), currentColor;
    protected float dodgelessTimer, dodgelessTimerRem, currentDuration, fraction, turnRed;
    protected bool isMoving = false, canDodge = true;    
    protected Collider2D triggerArea;

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
        moveToNextPos();
        checkDodgeSetting();
    }
    
    protected void moveToNextPos() {
        if (isMoving) {
            if (Vector2.Distance(transform.position, nextPos) > 0.1f) {
                transform.position = Vector2.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
            }
            else {
                isMoving = false;
                
            }
        }
    }

    // only dodge if ball enters trigger area
    protected void OnTriggerEnter2D (Collider2D collision) {
        if (isMoving || !canDodge) return; // no need to check if it's still moving or can't dodge yet
        if (collision.tag == "Ball") {
            if (canDodge) {
                Debug.Log("Target just dodged! ");
                // disable dodging
                generateNextPos();
                rbRender.material.color = baseColor;
                triggerArea.enabled = false;
                dodgelessTimer = Time.time + currentDuration; // enable dodge timer since it stopped moving now
                canDodge = false;
                isMoving = true;
            }
        }
    }

    protected override void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == "Ball") {
            destroyTarget();
        }
    }

    // reset dodge time
    protected void checkDodgeSetting() {
        if (!canDodge && !isMoving) {
            if (dodgelessTimer < Time.time) {
                Debug.Log("Target can now dodge! ");
                currentColor = new Color(1, 0, 0, 1);
                rbRender.material.color = currentColor;
                canDodge = true;
                triggerArea.enabled = true;
            }
            else changeTargetColors();
        }
        
    }
    
    // slowly turn red while the dodge timer is still ticking yet
    protected void changeTargetColors() {
        fraction = (Time.time - (dodgelessTimer - currentDuration)) / currentDuration;
        turnRed = Mathf.Lerp(0, 1 , fraction);
        currentColor = new Color(turnRed, baseColor.g, baseColor.b, 1);
        rbRender.material.color = currentColor;
    }

    // generate new position to move to
    protected void generateNextPos() {
        float xpos = Random.Range(minX, maxX);
        float ypos = Random.Range(minY, maxY);
        nextPos = new Vector2(xpos, ypos);
        rbRender.material.color = baseColor;
        currentColor = rbRender.material.color;
    }

    protected override void doOnApplyLevel() {
        // The currentDuration minimum range will start decreasing at 1 second per 20 levels, e.g. at level 80 onwards, random.range is (1, maxDuration);

        currentDuration = Random.Range(Mathf.Min(maxDuration, Mathf.Max(2, 100f / Level)), maxDuration);
        Debug.Log("Current Duration: " + currentDuration);
        rbRender = GetComponent<Renderer>();
        moveSpeed = 20f;
        generateNextPos();
        transform.position = nextPos;

        // set it to red since it can still dodge
        canDodge = true;
        isMoving = false;
        currentColor = new Color(1,0,0,1);
        rbRender.material.color = currentColor;

        // take the collider with the trigger area
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach (var collider in colliders) {
            Debug.Log("Searching trigger collider...");
            if (collider.isTrigger) {
                triggerArea = collider;
                Debug.Log("trigger Collider found: " + triggerArea);
                break;
            }
        }
    }

    protected override void doOnTimersSaved(bool state) {
        if (state) dodgelessTimerRem = dodgelessTimer - Time.time; // get the difference between the moveTimer's currentTime
        else dodgelessTimer = Time.time + dodgelessTimerRem; // apply the difference between the moveTimer's currentTime
    }

}
