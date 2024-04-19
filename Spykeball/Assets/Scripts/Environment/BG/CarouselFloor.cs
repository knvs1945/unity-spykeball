using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for moving the floor like a carousel
/// </summary>
public class CarouselFloor : GameUnit
{
    protected const float moveAmt = 0.2f;

    public Transform[] carouselPanels;
    public PlayerSpyke player;
    
    protected PlayerControls controls;
    protected Vector2 spawnPos;
    protected float leftLimit, rightLimit;
    protected int direction = 0;
    protected bool playerActive = false;
    

    // Start is called before the first frame update
    void Awake()
    {
        // get the base position of the outside frame. make sure it's at least 22 panels
        if (carouselPanels.Length >= 22) {
            spawnPos = carouselPanels[carouselPanels.Length - 1].position;
            rightLimit = spawnPos.x;
            spawnPos = carouselPanels[0].position;
            leftLimit = spawnPos.x;
            
            Debug.Log("Limits: " + leftLimit + " - " + rightLimit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == 0) return;
        getControls();

        getKeyPress();
        movePanels();
    }

    protected void getKeyPress() {
        if (!playerActive) return;
        bool moveLeft = (Input.GetKey(controls.MoveLeft));
        bool moveRight = (Input.GetKey(controls.MoveRight));

        if (moveLeft && moveRight) {
            direction = 0;
        }
        else if (moveLeft) {
            direction = 1; // reverse the directions to simulate parallax
        }
        else if (moveRight) {
            direction = -1;
        }
        else direction = 0;
    }

    protected void movePanels() {
        if (!playerActive) return;
        Vector2 pos, newPos;
        for (int i = 0; i < carouselPanels.Length; i++) {
            pos = carouselPanels[i].position;
            newPos = new Vector2(pos.x + (moveAmt * direction), pos.y);
            // check if newPosition breaches left or right limits
            if (direction == 1){
                if (newPos.x >= rightLimit) {
                    newPos = new Vector2(leftLimit, pos.y); // transfer the panel to the leftmost side
                } 
            }
            else if (direction == -1){
                if (newPos.x <= leftLimit) {
                    newPos = new Vector2(rightLimit, pos.y); // transfer the panel to the rightmost side
                } 
            }
            carouselPanels[i].position = newPos;
        }
    }

    // only get controls when player recently has them
    protected void getControls() {
        if (playerActive) return;

        Debug.Log("Controls check: " + controls);
        if (player != null) {
            controls = player.Controls;
            if (controls != null) {
                Debug.Log("Player found..." + controls);
                playerActive = true;
            }
        }
    }
}
