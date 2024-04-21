using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : Panel
{

    public Image titleCard;

    protected Animator titleAnim;
    
    void Awake()
    {
        if (titleCard != null) {
            titleAnim = titleCard.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void doOnPlayIntro() {
        if (titleAnim) titleAnim.SetTrigger("intro");
    }

    // animation event - played by title card animation
    public void playIntroSound() {
        SoundHandler.Instance.playSFX(SFXType.TimerSlam);
    }
}
