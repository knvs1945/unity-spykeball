using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera effect used to follow the player
/// </summary>
public class PlayerCam : MonoBehaviour
{
    public Transform player;
    public float offsetY = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x,player.transform.position.y + offsetY, -10f);
    }
}
