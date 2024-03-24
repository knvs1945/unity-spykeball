using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : Handler
{
    public float spawnBoundMinX, spawnBoundMaxX, spawnBoundMinY, spawnBoundMaxY;
    [SerializeField]
    protected List<Target> targetList;

    // Start is called before the first frame update
    void Start()
    {
        initializeStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register event behaviors
    protected void initializeStats() {
        Target.doOnDestroyTarget += spawnNewTarget;
    }

    protected void spawnNewTarget() {
        Debug.Log("Spawning new target here...");
        Target temp;
        Vector2 spawnPoint;
        spawnPoint = new Vector2( Random.Range(spawnBoundMinX, spawnBoundMaxX), Random.Range(spawnBoundMinY, spawnBoundMaxY) );
        temp = Instantiate(targetList[0], spawnPoint, Quaternion.identity);
    }
}
