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
