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
    public float moveAmt = 0.15f, panelGap = 0f;
    public bool enableDebug = false;
    
    protected PlayerControls controls;
    protected Vector2[] originalPos;
    protected Vector2 spawnPos;
    protected float leftLimit, rightLimit, panelWidth;
    protected int direction = 0;
    protected bool playerActive = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        int l = carouselPanels.Length;
        // get the base position of the outside frame. make sure it's at least 22 panels
        if (l > 0) {
            
            spawnPos = carouselPanels[l - 1].position;
            rightLimit = spawnPos.x;
            spawnPos = carouselPanels[0].position;
            leftLimit = spawnPos.x;
            
            panelWidth = carouselPanels[0].GetComponent<Renderer>().bounds.size.x;
            leftLimit -= (panelWidth / 2);
            rightLimit += (panelWidth / 2);

            // then get the original positions of the elements in the carousel
            originalPos = new Vector2[carouselPanels.Length];
            for (int i = 0; i < originalPos.Length; i++) {
                originalPos[i] = carouselPanels[i].position;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == 0) return;
        getControls();
    }

    void FixedUpdate()
    {
        if (gameState == 0) return;
        getKeyPress();
        movePanels();
    }

    // detects player keypress and adds movement data for movePanels accordingly
    protected void getKeyPress() {
        if (!playerActive) return;
        if (player.IsControlDisabled || isGamePaused) {
            direction = 0;
            return;
        }

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

    // moves panels according to key pressed
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
                    panelWidth = carouselPanels[i].GetComponent<Renderer>().bounds.size.x;
                    newPos = new Vector2(leftMostX - panelWidth + (moveAmt * direction) - panelGap, pos.y); // transfer the panel to the leftmost side
                    if (enableDebug) Debug.Log("Panel Data of Panel [" + i + "]: " + "Width: " + panelWidth + " - New Pos" + newPos);
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
                    panelWidth = carouselPanels[i].GetComponent<Renderer>().bounds.size.x;
                    newPos = new Vector2(rightMostX + panelWidth + (moveAmt * direction) + panelGap, pos.y); // transfer the panel to the rightmost side
                    if (enableDebug) Debug.Log("Panel Data of Panel [" + i + "]: " + "Width: " + panelWidth + " - New Pos" + newPos);
                }
                carouselPanels[i].position = newPos;
            }
        }
    }

    // only get controls when player recently has them
    protected void getControls() {
        if (playerActive) return;

        if (player != null) {
            controls = player.Controls;
            if (controls != null) {
                playerActive = true;
            }
        }
    }

    // resets the position of the carousel panels
    public void resetCarousel() {
        // then get the original positions of the elements in the carousel
        for (int i = 0; i < originalPos.Length; i++) {
            carouselPanels[i].position = originalPos[i];
        }
    }
}
