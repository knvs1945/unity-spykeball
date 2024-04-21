using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for handling environment and background objects
/// </summary>
public class EnvHandler : Handler
{
    private static EnvHandler Instance;
    public GameObject[] CrowdSet;

    public CarouselBG[] BGCrowd;
    public CarouselBG BGWall, BGFloor;
    
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
}
