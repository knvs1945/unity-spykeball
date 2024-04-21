using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{

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
