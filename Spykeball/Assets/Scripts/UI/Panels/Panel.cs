using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Panel class for UI panels like controls or menus
/// </summary>
public class Panel : MonoBehaviour
{
    public Button[] buttonSelection;

    public static PlayerControls controls = null;
    protected Animator titleAnim;
    protected int buttonIndex = 0;

    // Update is called once per frame
    protected virtual void Update()
    {
        highlightNextButton();
    }

    // highlight the first button in the buttonSelection when the panel is open
    protected void OnEnable() {
        buttonIndex = 0;
        if (buttonSelection.Length > 0) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonSelection[buttonIndex].gameObject);
        }
    }

    // highlight the next button on key press
    protected void highlightNextButton() {
        if (controls == null || buttonSelection.Length <= 0) return;
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

    public void playPanelIntro() {
        doOnPlayIntro();
    }

    public void playPanelOutro() {
        doOnPlayOutro();
    }

    public void setActive(bool value) {
        gameObject.SetActive(value);
    }

    // overrideables
    protected virtual void doOnPlayIntro() {}
    protected virtual void doOnPlayOutro() {}

}
