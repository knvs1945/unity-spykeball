using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // to access textmeshpro

/// <summary>
///  Effect for showing text
/// </summary>
public class ScoreEffect : GameUnit
{
    protected const float rateOfRise = 0.05f, rateOfFade = 0.025f;

    protected TextMeshPro textrnd;
    protected bool startEffect = false;

    void Awake()
    {
        textrnd = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        riseToFade();
    }

    protected void riseToFade() {
        if (!startEffect) return;

        Color temp = textrnd.color;
        float alpha = temp.a;
        Vector2 tempPos = transform.position;
        
        // start rising the effect
        tempPos.y = tempPos.y + rateOfRise;
        transform.position = tempPos;

        if (alpha > 0) {
            alpha -= rateOfFade;
            textrnd.color = new Color(temp.r, temp.g, temp.b, alpha);
        }
        else {
            Destroy(gameObject);
        }

    }

    public void addText(string _textToAdd ) {
        textrnd.text = _textToAdd;
        SoundHandler.Instance.playSFX(SFXType.ScoreTarget); // add scoretarget sound
        startEffect = true;
    }


}
