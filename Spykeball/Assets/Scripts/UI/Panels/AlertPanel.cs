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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void createPanel(string msg) {
        message.text = msg;
        EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    // button behavior here
    public void btClosePanel() {
        Destroy(gameObject);
    }
}
