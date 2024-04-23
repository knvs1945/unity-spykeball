using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for handling environment and background objects
/// </summary>
public class EnvHandler : Handler
{
    public static EnvHandler Instance;
    public CarouselBG[] BGCrowd;
    public CarouselBG BGWall, BGFloor;
    public GameObject[] CrowdSet;
    public GameObject panelDoor;
    
    private Animator doorAnim;
    
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        registerEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register event behaviors
    protected void registerEvents() {
        Target.doOnDestroyTarget += bgCrowdCheer;
        if (panelDoor) doorAnim = panelDoor.GetComponent<Animator>();
    }

    // restarts the envhandler and its items
    protected override void doOnRestartHandler() {
        BGFloor.resetCarousel();
        BGWall.resetCarousel();

        for (int i = 0; i < BGCrowd.Length; i++) {
            BGCrowd[i].resetCarousel();
        }
    }
    
    // makes the audience background animate a cheer when a target breaks
    protected void bgCrowdCheer() {
        Animator anim;
        for (int i = 0; i < CrowdSet.Length; i++) {
            anim = CrowdSet[i].GetComponent<Animator>();
            if (anim) {
                anim.SetTrigger("cheer");
            }
        }
    }

    // misc - open the panel door on intro
    public void introDoorOpen() {
        doorAnim.SetTrigger("open");
    }
}
