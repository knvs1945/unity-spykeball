using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class for the control panel
public class ControlPanel : Panel
{
    // delegates and events
    public delegate void closeSettings(bool save);
    public static event closeSettings doOnCloseSettings;

    public Slider trackSlider, SFXSlider;

    [SerializeField]
    protected Button[] controlSetUI;

    [SerializeField]
    protected Text[] btText;

    protected string[] newControls;
    protected string currentButton = "";
    protected bool isListening = false;


    public string[] NewControls {
        get { return newControls; }
    }


    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        listenToControlUpdates();
    }
    public void setCurrentControlUI(PlayerControls controls) {
        if (btText.Length > 0) {
            btText[0].text = controls.MoveLeft.ToUpper();
            btText[1].text = controls.MoveRight.ToUpper();
            btText[2].text = controls.MoveUp.ToUpper();
            btText[3].text = controls.MoveDown.ToUpper();
            btText[4].text = controls.Attack.ToUpper();
        }

        newControls = controls.getControlSet();   
    }

    // get current controls from the UI handler data passed from the player handler
    public PlayerControls getCurrentControl() {
        PlayerControls newControl = new PlayerControls(newControls);
        return newControl;
    }

    // get the current volumes from the sound handler
    public void setCurrentVolumeUI(float[] values) {
        if (values.Length >= 2) {
            SFXSlider.value = values[0];
            trackSlider.value = values[1];
        }
    }

    public float[] getCurrentVolumeUI() {
        float[] values = new float[] {SFXSlider.value, trackSlider.value};
        return values;
    }

    /// <summary>
    /// 
    /// settings panel button behavior here
    /// 
    /// </summary>
    public void btCloseSettingsWithoutSave() {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnCloseSettings(false);
    }

    public void btControlButtonClicked(string buttonClicked) {
        currentButton = buttonClicked;
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        isListening = true;
    }

    // apply changes clicked
    public void btControlConfirmChanges() {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnCloseSettings(true);
    }

    /// <summary>
    /// 
    /// Listen for control updates here
    /// 
    /// </summary>
    protected void listenToControlUpdates() {
        if (!isListening) return;
        int controlindex = 0, UIindex = 0;
        
        // get control index of the current button clicked
        switch(currentButton) {
            case "left": UIindex = 0;
                         controlindex = 2;
                         break;
            case "right": UIindex = 1;
                          controlindex = 3;
                          break;
            case "jump": UIindex = 2;
                         controlindex = 0;
                         break;
            case "dash": UIindex = 3;
                         controlindex = 1;
                         break;
            case "spike": UIindex = 4;
                          controlindex = 4;
                          break;
        }

        // capture keys here
        if (Input.anyKeyDown) {
            bool keyChanged = false;
            string keyPresses = Input.inputString; // get all the keys prseed during the time
            if (!string.IsNullOrEmpty(keyPresses) && keyPresses.ToLower() != "escape") {
                keyPresses = filterSpecialKeyPress(keyPresses[0].ToString().ToLower());
                newControls[controlindex] = keyPresses; // just get the first character recorded    
                keyChanged = true;
                // btText[UIindex].text = newControls[controlindex].ToString().ToUpper();
            }
            // handle non-character special keys pressed here - just show warning if special keys are selected
            else if (Input.GetKeyDown(KeyCode.LeftShift)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.LeftShift);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightShift)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.RightShift);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftAlt)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.LeftAlt);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightAlt)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.RightAlt);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightAlt)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.RightAlt);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.LeftControl);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightControl)) {
                UIHandler.createModal("warning", "Special Keys are not supported: " + KeyCode.RightControl);
                keyChanged = true;
            }
            // arrow keys
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                UIHandler.createModal("warning", "Arrow Keys are already supported by default: " + KeyCode.RightArrow);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                UIHandler.createModal("warning", "Arrow Keys are already supported by default: " + KeyCode.LeftArrow);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                UIHandler.createModal("warning", "Arrow Keys are already supported by default: " + KeyCode.UpArrow);
                keyChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                UIHandler.createModal("warning", "Arrow Keys are already supported by default: " + KeyCode.DownArrow);
                keyChanged = true;
            }
            else if (keyPresses.ToLower() == "escape") {
                Debug.Log("Canceling button update");
            }

            if (keyChanged) btText[UIindex].text = newControls[controlindex].ToString().ToUpper();
            isListening = false;
        }
    }

    // filter for any special non alphanumeric key pressed
    protected string filterSpecialKeyPress(string keypressed) {
        string newKey = keypressed;
        switch(keypressed) {
            // check if spacebar
            case " ": newKey = "space";
                      break;
            case "\r": newKey = "enter";
                      break;
            case "\b": newKey = "backspace";
                      break;
        }
        return newKey;
    }




}
