using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handler for generic in-game effects
/// </summary>
public class EffectHandler : Handler
{
    
    public static EffectHandler Instance {get; private set; }
    
    public ImpactRing VFXImpactRing;
    public SpeedMirage VFXSpeedMirage;
    
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // create impact rings from spiking or hard attacks
    public void CreateEffectImpactRing(Vector2 location, float rateOfFade) {
        Debug.Log("Creating impact ring...");
        ImpactRing temp = Instantiate(VFXImpactRing, location, Quaternion.identity);
        if (temp != null) temp.applyEffect(rateOfFade);
    }

    // create speed effect for dashes and speedups
    public void CreateEffectSpeedMirage(Vector2 location, Sprite _sprite) {
        SpeedMirage temp = Instantiate(VFXSpeedMirage, location, Quaternion.identity);
        if (temp != null) temp.applySprite(_sprite);
    }
}
