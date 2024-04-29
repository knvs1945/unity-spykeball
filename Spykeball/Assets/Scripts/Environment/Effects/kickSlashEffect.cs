using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kick Slash effect
/// </summary>
public class KickSlashEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void slashComplete() {
        Destroy(gameObject);
    }
}
