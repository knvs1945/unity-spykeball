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
    TargetBreak,
    ScoreTarget,
    ButtonClick,
    TimerSlam
}

/// <summary>
/// Handler for soundtracks and sound effects
/// </summary>
public class SoundHandler : Handler
{
    public static SoundHandler Instance {get; private set; }

    public AudioClip[] SFXEntries;
    public AudioClip[] TrackEntries;

    public AudioSource[] SFXSources;
    public AudioSource Tracks;

    protected float SFXVolume = 1, TrackVolume = 1;

    void Awake()
    {
        // make sure only one instance of this handler is present
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
        TrackVolume = Tracks.volume;
        SFXVolume = SFXSources[0].volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void restartHandler() {
        for (int i = 0; i < SFXSources.Length; i++) {
            SFXSources[i].Stop();
        }
        playGameTrack();
    }

    public override void returnToMainMenu() {
        for (int i = 0; i < SFXSources.Length; i++) {
            SFXSources[i].Stop();
        }
        Tracks.Stop();
    }

    // sfx tracks here
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

    public void playGameTrack(int trackId = 0, bool isLooped = true) {
        if (gameState == states.inStage) {
            if (trackId < TrackEntries.Length) {
                Tracks.Stop();
                Tracks.clip = TrackEntries[trackId];
                Tracks.loop = isLooped;
                Tracks.Play();
            }
        }
    }

    public void stopGameTrack() {
        Tracks.Stop();
    }

    // volume setters and getters
    public float[] getVolumeValues() {
    
        return new float[] {SFXVolume, TrackVolume};
    }

    public void setVolumeValues(float[] newVolumes) {
        if (newVolumes.Length < 2) return;
        SFXVolume = newVolumes[0];
        TrackVolume = newVolumes[1];
        applyVolumeValues();
    }

    // apply new volumes set from the UIHandler's control panel
    protected void applyVolumeValues() {
        Tracks.volume = TrackVolume;

        for (int i = 0; i < SFXSources.Length; i++) {
            SFXSources[i].volume = SFXVolume;
        }
    }
}
