using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect for creating mirages during fast movement
/// </summary>
public class SpeedMirage : GameUnit
{
    protected const float fadeDuration = 1f, rateOfFade = 0.05f;

    protected SpriteRenderer rnd;
    protected Color matcolor;
    protected bool startFade = false;

    void Awake()
    {
        rnd = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fadeOut();
    }

    protected void fadeOut() {
        if (!startFade) return; // don't fade unless initiated

        // get current alpha of the renderer
        Color temp = rnd.material.color;
        float alpha = temp.a;
        
        // apply decreasing alpha 
        if (alpha >= 0) {
            alpha -= rateOfFade;
            matcolor = new Color(temp.r,temp.g,temp.b, alpha);
            rnd.material.color = matcolor;
        }
        else {
            Destroy(gameObject); // destroy the effect since it's faded out now
        }
    }

    public void applySprite(Sprite _sprite) {
        rnd.sprite = _sprite;
        startFade = true; // we can start fading out now
    }

}
