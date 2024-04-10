using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Effect for making an impact ring
/// </summary>
public class ImpactRing : GameUnit
{
    protected const float rateOfResizeHeight = 0.5f, rateOfResizeWidth = 3f;
    protected float rateOfFade = 0.03f; // default to 0.05f

    protected SpriteRenderer rnd;
    protected bool startResize = false;

    void Awake()
    {
        rnd = GetComponent<SpriteRenderer>();    
    }

    // Update is called once per frame
    void Update()
    {
        resizeToFade();
    }

    protected void resizeToFade() {
        if (!startResize) return;

        Color temp = rnd.material.color;
        Vector2 scale = transform.localScale;
        float alpha = temp.a;

        // resize the impact ring
        scale.x += rateOfResizeWidth;
        scale.y += rateOfResizeHeight;
        transform.localScale = scale;

        if (alpha >= 0) {
            alpha -= rateOfFade;
            rnd.material.color = new Color(temp.r,temp.g, temp.b, alpha);
        }
        else {
            Destroy(gameObject); // destroy impact item
        }        
    }
    
    public void applyEffect(float _rateOfFade) {
        SoundHandler.Instance.playSFX(SFXType.SpikeHitHard);
        rateOfFade = _rateOfFade;
        startResize = true;
    }
}
