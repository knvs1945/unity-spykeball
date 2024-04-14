using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionHandler : Handler
{
    private static TransitionHandler Instance;
    
    public GameObject transitionPanel;
    public Camera mainCamera;

    protected Vector2 panelStartPos, panelCoverPos;
    protected Animator tAnim;

    void Start() {
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (transitionPanel != null) {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(new Vector3(0, 0, 0));
                Vector3 outPos = mainCamera.WorldToScreenPoint(transitionPanel.transform.position);
                
                panelStartPos = new Vector2(outPos.x, outPos.y);
                panelCoverPos = new Vector2(screenPos.x, screenPos.y);
                
                tAnim = transitionPanel.GetComponent<Animator>();
            }
        }
        else Destroy(gameObject);
    }

    // cover the game while doing transitions
    public void startStateTransition() {
        Debug.Log("Starting Transition: " + tAnim);
        tAnim.SetTrigger("start");
    }

    // uncover the game after a transition
    public void endStateTransition() {
        tAnim.SetTrigger("end");
    }

    public void doOnTransitionDone() {

    }
}
