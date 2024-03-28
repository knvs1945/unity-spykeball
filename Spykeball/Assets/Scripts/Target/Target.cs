using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Floating Target Classes
public class Target : GameUnit
{
    protected const float finalSpeed = 25f;

    // Delegates and Events
    public delegate void onDestroyTarget();
    public static event onDestroyTarget doOnDestroyTarget;

    // explosion effect
    public ParticleSystem targetBreak;

    protected Rigidbody2D rb;
    // protected int Level = 1;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    protected virtual void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == "Ball") {
            destroyTarget();
        }
    }

    // destroy and respawn new target. Set noSpawn to true if the targets don't need to spawn
    protected void destroyTarget(bool noRespawn = false) {
        if (!noRespawn) {
            Instantiate(targetBreak, transform.position, Quaternion.identity);
            doOnDestroyTarget();
        }
        Destroy(gameObject);
    }
    
    // restart target spawn by destroying the current one
    public void restartTarget() {
        destroyTarget(true);
    }

    public void applyLevel(int level = 1) {
        Level = level;
        doOnApplyLevel();
    }

    protected virtual void doOnApplyLevel() {

    }


}
