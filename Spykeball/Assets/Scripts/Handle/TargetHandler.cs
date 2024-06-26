using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : Handler
{
    protected const int levelUp = 3; // increase target choices based on every N levels of this constant
    public float spawnBoundMinX, spawnBoundMaxX, spawnBoundMinY, spawnBoundMaxY;
    [SerializeField]
    protected List<Target> targetList;

    protected Target currentTarget;

    // Start is called before the first frame update
    void Awake()
    {
        registerEvents();
        // doOnRestartHandler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register event behaviors
    protected void registerEvents() {
        Target.doOnDestroyTarget += spawnNewTarget;
    }

    public override void returnToMainMenu() {
        // hide the current target
        if (currentTarget != null) currentTarget.restartTarget();
    }

    protected override void doOnRestartHandler() {
        if (currentTarget != null) currentTarget.restartTarget();
        spawnNewTarget();
    }

    // spawn targets here
    protected void spawnNewTarget() {
        Target temp;
        Vector2 spawnPoint;
        int index = 0, maxRange;

        // generate a target randomly but gradually based on game level
        maxRange = Mathf.Min(targetList.Count, (gameLevel / levelUp) + 1);
        index = Random.Range(0, maxRange);        

        spawnPoint = new Vector2( Random.Range(spawnBoundMinX, spawnBoundMaxX), Random.Range(spawnBoundMinY, spawnBoundMaxY) );
        temp = Instantiate(targetList[index], spawnPoint, Quaternion.identity);
        if (temp != null) {
            temp.applyLevel(gameLevel);
            currentTarget = temp;
        }
    }


}
