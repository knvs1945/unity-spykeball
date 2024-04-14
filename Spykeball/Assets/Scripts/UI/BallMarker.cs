using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// class for checking where the ball 
public class BallMarker : MonoBehaviour
{
    protected const float screenYBounds = 5.75f;
    
    public GameObject ball;
    public Camera mainCamera;
    public RectTransform marker;

    protected Image image;
    protected Transform bt;
    protected float fixedY, screenX;
    protected bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        showMarker(false); // hide the marker by default
    }

    void Awake()
    {
        if (ball != null) bt = ball.transform;
        image = gameObject.GetComponent<Image>();
        fixedY = marker.anchoredPosition.y;
        screenX = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (ball != null) {
            trackBall();
        }
    }

    // tracks the ball's movement and show it on the UI if it is above a certain threshold
    protected void trackBall() {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(bt.position); // convert the ball's position;
        marker.anchoredPosition = new Vector2(screenPos.x - screenX, fixedY);

        if (bt.position.y > screenYBounds) {
            showMarker(true);
            changeMarkerColor(bt.position.y);
        }
        else showMarker(false);
    }

    // set to true to show marker, false to hide
    protected void showMarker(bool value) {
        Color display = image.color;
        if (isShowing && !value) {
            display.a = 0;
            isShowing = false;
        }
        else if (!isShowing && value) {
            display.a = 1;
            isShowing = true;
        }
        image.color = display;
    }

    // change marker color depending on how far the ball is off-screen
    protected void changeMarkerColor(float distance) {
        Color display = image.color;
        float rbValue = 0;
        // divide the distance difference by 3 to adjust for the world coordinates
        rbValue = Mathf.Min((distance - screenYBounds)/3, 1);
        display.r = rbValue;
        display.b = rbValue;
        image.color = display;
    }
}
