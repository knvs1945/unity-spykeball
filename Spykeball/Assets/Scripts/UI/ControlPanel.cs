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

    protected char[] newControls;
    protected string currentButton = "";
    protected bool isListening = false;

    void Awake()
    {
        newControls = new char[btText.Length];
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

        for (int i = 0; i < btText.Length; i++) {
            newControls[i] = btText[i].text[0];
        }
        
    }

    // settings panel button behavior here
    public void btCloseSettingsWithoutSave() {
        doOnCloseSettings(false);
    }

    public void btControlButtonClicked(string buttonClicked) {
        Debug.Log("Setting button clicked: " + buttonClicked);
        currentButton = buttonClicked;
        isListening = true;
    }

    // Listen for control updates here
    protected void listenToControlUpdates() {
        if (!isListening) return;
        int index = 0;

        if (Input.anyKeyDown) {
            string keyPresses = Input.inputString; // get all the keys prseed during the time
            if (!string.IsNullOrEmpty(keyPresses)){
                if (keyPresses.ToLower() != "escape") {        
                    switch(currentButton) {
                        case "left": index = 0;
                                     break;
                        case "right": index = 1;
                                     break;
                        case "jump": index = 2;
                                     break;
                        case "dash": index = 3;
                                     break;
                        case "spike": index = 4;
                                     break;
                    }
                    newControls[index] = keyPresses[0]; // just get the first character recorded
                    btText[index].text = newControls[index].ToString().ToUpper();
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
