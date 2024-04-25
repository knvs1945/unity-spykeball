using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitlePanel : Panel
{
    public Image titleCard;
    public Button[] buttonSelection;

    public PlayerControls controls;
    protected Animator titleAnim;
    protected int buttonIndex = 0;
    
    void Awake()
    {
        if (titleCard != null) {
            titleAnim = titleCard.GetComponent<Animator>();
            buttonIndex = 0;
        }
    
    }


    // Update is called once per frame
    void Update()
    {
        highlightNextButton();
    }

    // highlight the next button on key press
    protected void highlightNextButton() {
        if (controls == null) return;
        bool buttonPressed = false;

        if (Input.GetKeyDown(controls.Attack)) {
            buttonSelection[buttonIndex].onClick.Invoke();
        }
        if (Input.GetKeyDown(controls.MoveDown)) {
            buttonPressed = true;
            buttonIndex++;
        }
        else if (Input.GetKeyDown(controls.MoveUp)) {
            buttonPressed = true;
            buttonIndex--;
        }

        if (buttonPressed) {
            if (buttonIndex >= buttonSelection.Length) buttonIndex = 0;
            else if (buttonIndex < 0) buttonIndex = buttonSelection.Length - 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonSelection[buttonIndex].gameObject);

            Debug.Log("highlighting next button: " + buttonIndex + " - " + buttonSelection[buttonIndex].gameObject);
        }
    }

    protected override void doOnPlayIntro() {
        if (titleAnim) titleAnim.SetTrigger("intro");
    }

    // animation event - played by title card animation
    public void playIntroSound() {
        SoundHandler.Instance.playSFX(SFXType.TimerSlam);
    }

}
