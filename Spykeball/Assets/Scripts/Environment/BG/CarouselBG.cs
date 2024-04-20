using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script for moving objects like a carousel
/// </summary>
public class CarouselBG : GameUnit
{
    public Transform[] carouselPanels;
    public PlayerSpyke player;
    public float moveAmt = 0.15f;
    
    protected PlayerControls controls;
    protected Vector2 spawnPos;
    protected float leftLimit, rightLimit, panelWidth;
    protected int direction = 0;
    protected bool playerActive = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        
        // get the base position of the outside frame. make sure it's at least 22 panels
        if (carouselPanels.Length > 0) {
            
            spawnPos = carouselPanels[carouselPanels.Length - 1].position;
            rightLimit = spawnPos.x;
            spawnPos = carouselPanels[0].position;
            leftLimit = spawnPos.x;
            
            panelWidth = carouselPanels[0].GetComponent<Renderer>().bounds.size.x;
            leftLimit -= (panelWidth / 2);
            rightLimit += (panelWidth / 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == 0) return;
        getControls();

        getKeyPress();
    }

    void FixedUpdate()
    {
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
        if (direction == 1){
            for (int i = 0; i < carouselPanels.Length; i++) {
                pos = carouselPanels[i].position;
                newPos = new Vector2(pos.x + (moveAmt * direction), pos.y);
                // check if newPosition breaches left or right limits
                if (newPos.x > rightLimit) {
                    float leftMostX = carouselPanels.Min(panel => panel.position.x);
                    newPos = new Vector2(leftMostX - panelWidth + (moveAmt * direction), pos.y); // transfer the panel to the leftmost side
                } 
                carouselPanels[i].position = newPos;
            }
        }
        else if (direction == -1){
            // reverse the loop order to prevent gap bug
            for (int i = carouselPanels.Length - 1; i >= 0; i--) {
                pos = carouselPanels[i].position;
                newPos = new Vector2(pos.x + (moveAmt * direction), pos.y);
                if (newPos.x < leftLimit) {
                    float rightMostX = carouselPanels.Max(panel => panel.position.x);
                    newPos = new Vector2(rightMostX + panelWidth + (moveAmt * direction), pos.y); // transfer the panel to the rightmost side
                }
                carouselPanels[i].position = newPos;
            }
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
