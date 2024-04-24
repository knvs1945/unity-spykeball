using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Target slowly disappears before popping somewhere else
public class Target4 : Target
{
    public float minX, maxX, minY, maxY, maxDuration;

    protected Renderer rbRender;
    protected Vector2 nextPos;
    protected Color baseColor = new Color(1,1,1,1), currentColor;
    protected float moveTimer, moveTimerRem, currentDuration, fraction, newAlpha;
    

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
        fadeOutToNextPos();
    }

    protected void fadeOutToNextPos() {
        if (moveTimer > Time.time) {
            fraction = (Time.time - (moveTimer - currentDuration)) / currentDuration;
            newAlpha = Mathf.Lerp(1, 0 , fraction);
            currentColor = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);
            rbRender.material.color = currentColor;
        }
        else {
            generateNextPos();
            moveTimer = Time.time + currentDuration;
        }
    }

    // generate new position to move to
    protected void generateNextPos() {
        float xpos = Random.Range(minX, maxX);
        float ypos = Random.Range(minY, maxY);
        nextPos = new Vector2(xpos, ypos);
        transform.position = nextPos;
        // redo flashing intro
        gameObject.GetComponent<Animator>().Play("target_intro");
        
        rbRender.material.color = baseColor;
        currentColor = rbRender.material.color;
    }

    protected override void doOnApplyLevel() {
        // The currentDuration minimum range will start decreasing at 1 second per 20 levels, e.g. at level 80 onwards, random.range is (1, maxDuration);
        currentDuration = Random.Range(Mathf.Min(maxDuration, Mathf.Max(1, 100f / Level)), maxDuration);
        Debug.Log("Current Duration: " + currentDuration);
        rbRender = GetComponent<Renderer>();
        generateNextPos();
        moveTimer = Time.time + currentDuration;
    }

    protected override void doOnTimersSaved(bool state) {
        if (state) moveTimerRem = moveTimer - Time.time; // get the difference between the moveTimer's currentTime
        else moveTimer = Time.time + moveTimerRem; // apply the difference between the moveTimer's currentTime
    }
}
