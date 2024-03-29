using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : Handler
{
    public float spawnBoundMinX, spawnBoundMaxX, spawnBoundMinY, spawnBoundMaxY;
    [SerializeField]
    protected List<Target> targetList;

    protected Target currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        initializeStats();
        // doOnRestartHandler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register event behaviors
    protected void initializeStats() {
        Target.doOnDestroyTarget += spawnNewTarget;
    }

    protected override void doOnRestartHandler() {
        Debug.Log("Restarting Target handler");
        if (currentTarget != null) currentTarget.restartTarget();
        spawnNewTarget();
    }

    // spawn targets here
    protected void spawnNewTarget() {
        Debug.Log("Spawning new target here...");
        Target temp;
        Vector2 spawnPoint;
        spawnPoint = new Vector2( Random.Range(spawnBoundMinX, spawnBoundMaxX), Random.Range(spawnBoundMinY, spawnBoundMaxY) );
        temp = Instantiate(targetList[0], spawnPoint, Quaternion.identity);
        if (temp != null) {
            temp.applyLevel(1);
            currentTarget = temp;

        }
    }


}
