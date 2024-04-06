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


    [SerializeField]
    protected Button[] controlSetUI;

    [SerializeField]
    protected Text[] btText;

    protected string[] newControls;
    protected string currentButton = "";
    protected bool isListening = false;

    public string[] NewControls {
        get {return newControls; }
    }

    void Awake()
    {
        newControls = new string[btText.Length];
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

    public PlayerControls getCurrentControl() {
        PlayerControls newControl = new PlayerControls(newControls);
        Debug.Log("New Controls: " + newControl.MoveLeft + "-" + newControl.MoveRight + "-" + newControl.MoveUp + "-" + newControl.MoveDown + "-" + newControl.Attack);
        return newControl;
    }

    /// <summary>
    /// 
    /// settings panel button behavior here
    /// 
    /// </summary>
    public void btCloseSettingsWithoutSave() {
        doOnCloseSettings(false);
    }

    public void btControlButtonClicked(string buttonClicked) {
        Debug.Log("Setting button clicked: " + buttonClicked);
        currentButton = buttonClicked;
        isListening = true;
    }

    // apply changes clicked
    public void btControlConfirmChanges() {
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

        if (Input.anyKeyDown) {
            string keyPresses = Input.inputString; // get all the keys prseed during the time
            if (!string.IsNullOrEmpty(keyPresses)){
                if (keyPresses.ToLower() != "escape") {        
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
                    newControls[controlindex] = keyPresses[0].ToString().ToLower(); // just get the first character recorded
                    btText[UIindex].text = newControls[controlindex].ToString().ToUpper();
                    isListening = false;
                }
                else {
                    Debug.Log("Canceling button update");
                    isListening = false;
                }
            }
        }

    }



}
