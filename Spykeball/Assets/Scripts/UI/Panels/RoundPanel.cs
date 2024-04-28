using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel that plays when a round starts
/// </summary>
public class RoundPanel : Panel
{
    public Image caption, subcaption;
    public Sprite[] unliCaptions, ttCaptions;
    void Awake() {
        titleAnim = GetComponent<Animator>();
    }

    // overrideables
    protected override void doOnPlayIntro() {
        if (gameMode == "Survival") {
            caption.sprite = unliCaptions[0];
            subcaption.sprite = unliCaptions[1];
        }
        else if (gameMode == "Time Attack") {
            caption.sprite = ttCaptions[0];
            subcaption.sprite = ttCaptions[1];
        }
        titleAnim.SetTrigger("round_unli");
    }

    public void introDone() {
        gameObject.SetActive(false);
    }


}
