using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    Bounce,
    Dash,
    Spike,
    SpikeHit,
    SpikeHitHard,
    ScoreTarget
}

/// <summary>
/// Handler for soundtracks and sound effects
/// </summary>
public class SoundHandler : Handler
{
    public static SoundHandler Instance {get; private set; }

    public AudioClip[] SFXEntries;
    public AudioSource[] SFXSources;
    public AudioSource Tracks;

    void Awake()
    {
        // make sure only one instance of this handler is present
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

    // called by gameobjects who want to play a sound effect
    public void playSFX(SFXType SFXName) {
        AudioClip sfxToPlay = SFXEntries[(int) SFXName];
        checkForOpenSFX(sfxToPlay);
    }

    // only play an sfx if it's available
    protected void checkForOpenSFX(AudioClip sfxToPlay) {
        for (int i = 0; i < SFXSources.Length; i++) {
            if (!SFXSources[i].isPlaying) {
                SFXSources[i].clip = sfxToPlay;
                SFXSources[i].Play();
                break;
            }
        }
    }
}
