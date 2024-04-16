using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Effect for showing text like an alert in the background, for the timer
/// </summary>
public class AlertTextEffect : GameUnit
{
    protected const float rateOfResize = 10f, rateOfFadeIn = 0.1f;

    protected TextMeshPro textrnd;
    protected RectTransform rt;
    protected float finalSize;
    protected bool startEffect = false, startFadeOut = false;
    
    void Awake()
    {
        textrnd = GetComponent<TextMeshPro>();
        rt = GetComponent<RectTransform>();
        finalSize = textrnd.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        showAndSizeText();
    }

    protected void showAndSizeText() {
        if (!startEffect) return;

        Color temp = textrnd.color;
        float curSize = textrnd.fontSize;
        float alpha = temp.a;
        if (!startFadeOut) {
            if (curSize > finalSize) {
                curSize -= rateOfResize;
                textrnd.fontSize = curSize;
                alpha += rateOfFadeIn;
                textrnd.color = new Color(temp.r, temp.g, temp.b, alpha);
            }
            else {
                textrnd.fontSize = finalSize;
                SoundHandler.Instance.playSFX(SFXType.TimerSlam);
                startFadeOut = true;
            }
        }

        if (!startFadeOut) return; // only start fading out after resizing
        if (alpha > 0) {
            alpha -= rateOfFadeIn;
            textrnd.color = new Color(temp.r, temp.g, temp.b, alpha);
        }
        else {
            Destroy(gameObject);
        }

    }

    public void addText(string _textToAdd) {
        textrnd.text = _textToAdd;
        float startSize = 200f;
        textrnd.fontSize = startSize;

        // start the effect as transparent
        Color temp = textrnd.color;
        textrnd.color = new Color(temp.r, temp.g, temp.b, 0);
        startEffect = true;
    }
}
