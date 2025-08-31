using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ScoreSet
{
    public string name, date, time;
    public int score, targets;
}

[System.Serializable]
public class HSList
{
    public ScoreSet[] highscores;
    public int length;
}

/// <summary>
/// Panel class for UI panels like controls or menus
/// </summary>
public class Panel : MonoBehaviour
{
    // string constants for child panels inheriting from Panel class
    protected const string LOCAL_READHS = "http://localhost:5000/sbreadhs?";
    protected const string LOCAL_UPDATEHS = "http://localhost:5000/sbupdatehs";

    // protected const string LIVE_READHS = "https://bitknvs-30e00398cef5.herokuapp.com/sbreadhs?";
    // protected const string LIVE_UPDATEHS = "https://bitknvs-30e00398cef5.herokuapp.com/sbupdatehs";
    
    protected const string LIVE_READHS = "https://bitknvs.com/sbreadhs?";
    protected const string LIVE_UPDATEHS = "https://bitknvs.com/sbupdatehs";

    protected const string GAMEMODEUL = "/ul";
    protected const string GAMEMODETA = "/ta";
    protected static bool preventNavigation = false;

    public Button[] buttonSelection;

    public static PlayerControls controls = null;
    public static string gameMode;
    protected static bool modalActive = false;

    protected Animator titleAnim;
    protected int buttonIndex = 0;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!preventNavigation && !modalActive) highlightNextButton();
    }

    // highlight the first button in the buttonSelection when the panel is open
    protected virtual void OnEnable() {
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

        if (Input.GetKeyDown(controls.Attack) || Input.GetKeyDown(KeyCode.Space)) {
            buttonSelection[buttonIndex].onClick.Invoke();
        }
        else if (Input.GetKeyDown(controls.MoveDown) || Input.GetKeyDown(controls.MoveRight)) {
            buttonPressed = true;
            buttonIndex++;
        }
        else if (Input.GetKeyDown(controls.MoveUp) || Input.GetKeyDown(controls.MoveLeft)) {
            buttonPressed = true;
            buttonIndex--;
        }

        if (buttonPressed) {
            if (buttonIndex >= buttonSelection.Length) buttonIndex = 0;
            else if (buttonIndex < 0) buttonIndex = buttonSelection.Length - 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonSelection[buttonIndex].gameObject);
            buttonPressed = false;
            // Debug.Log("highlighting next button: " + buttonIndex + " - " + buttonSelection[buttonIndex].gameObject);
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
