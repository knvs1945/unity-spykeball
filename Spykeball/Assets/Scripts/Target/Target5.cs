using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dodges the ball for N amount of times
public class Target5 : Target
{
    protected const float spdEffectGap = 0.05f;
    public float minX, maxX, minY, maxY;
    public int maxDodges;

    protected SpriteRenderer spriteRnd;
    protected Vector2 nextPos;
    protected float currentDodges, spdEffectTimer;
    protected bool isMoving = false;

    // Update is called once per frame
    void Update()
    {
        if (checkIfGamePaused()) return;
        moveToNextPos();
    }

    protected void moveToNextPos() {
        if (isMoving) {
            createSpdEffects();
            if (Vector2.Distance(transform.position, nextPos) > 0.1f) {
                transform.position = Vector2.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
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
                generateNextPos();
                isMoving = true;
                // start the effect timer
                spdEffectTimer = Time.time + spdEffectGap;
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
        spriteRnd = GetComponent<SpriteRenderer>();
        currentDodges = maxDodges + (int) Mathf.Min(3, Level/12);
        Debug.Log("Dodges Set: " + currentDodges);
        moveSpeed = 20f;
        generateNextPos();
    }

    protected void createSpdEffects() {
        if (!isMoving) return;
        if (Time.time < spdEffectTimer) return;
            
        EffectHandler.Instance.CreateEffectSpeedMirage(transform.position, spriteRnd.sprite, false, Color.magenta);
        spdEffectTimer = Time.time + spdEffectGap;
    }
}
