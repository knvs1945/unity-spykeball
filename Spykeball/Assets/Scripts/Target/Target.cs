using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Floating Target Classes
public class Target : GameUnit
{
    // Delegates and Events
    public delegate void onDestroyTarget();
    public static event onDestroyTarget doOnDestroyTarget;

    // explosion effect
    public ParticleSystem targetBreak;

    protected Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnCollisionEnter2D (Collision2D collision) {
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
}
