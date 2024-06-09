using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlertPanel : Panel
{

    [SerializeField]
    protected Button closeButton;

    [SerializeField]
    protected Text message;


    protected override void Update()
    {
        // player has clicked on attack or spyke button
        if (Input.GetKeyDown(controls.Attack) || Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Pressing Attack");
            closeButton.onClick.Invoke();
        }
    }

    public void createPanel(string msg) {
        if (modalActive) {

            return; // only 1 modal can be active at a time
        }
        message.text = msg;
        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        modalActive = true;
    }

    // button behavior here
    public void btClosePanel() {
        modalActive = false;
        Destroy(gameObject);
    }
}
