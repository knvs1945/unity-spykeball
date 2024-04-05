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

    protected bool isListening = false;

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
    }

    // settings panel button behavior here
    public void btCloseSettingsWithoutSave() {
        doOnCloseSettings(false);
    }

    public void btControlButtonClicked() {

    }

    // Listen for control updates here
    protected void listenToControlUpdates() {
        if (!isListening) return;
    }



}
