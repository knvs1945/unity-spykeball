using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBreak : GameUnit
{
    // Start is called before the first frame update
    void Start()
    {
        // play effect when started
        SoundHandler.Instance.playSFX(SFXType.TargetBreak);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
