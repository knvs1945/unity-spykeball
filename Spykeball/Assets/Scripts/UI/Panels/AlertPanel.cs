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

    protected float timer = 0;
    bool isTimed = false;


    protected override void Update()
    {
        // player has clicked on attack or spyke button
        if (Input.GetKeyDown(controls.Attack) || Input.GetKeyDown(KeyCode.Space)) {
            closeButton.onClick.Invoke();
        }

        // close modal if timed
        if (isTimed && timer < Time.time) {
            btClosePanel();
        }
    }

    public void createPanel(string msg, float timeout = 0) {
        if (modalActive) return; // only 1 modal can be active at a time
        message.text = msg;
        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        modalActive = true;

        if (timeout > 0) {
            isTimed = true;
            timer = Time.time + timeout;
        }
    }

    // button behavior here
    public void btClosePanel() {
        modalActive = false;
        Destroy(gameObject);
    }
}
