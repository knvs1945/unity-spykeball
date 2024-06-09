using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : Panel
{
    public Image titleCard;
    
    void Awake()
    {
        if (titleCard != null) {
            titleAnim = titleCard.GetComponent<Animator>();
            buttonIndex = 0;
        }
    
    }

    protected override void doOnPlayIntro() {
        if (titleAnim) titleAnim.SetTrigger("intro");
    }

    // animation event - played by title card animation
    public void playIntroSound() {
        SoundHandler.Instance.playSFX(SFXType.TimerSlam);
    }

    // button behavior here
    public void btCreditsClicked() {
        UIHandler.createModal("confirm", " Coding: KJ Cabrera, \r\n Art: KJ Cabrera, \r\n Music: Elkan");
    }

}
