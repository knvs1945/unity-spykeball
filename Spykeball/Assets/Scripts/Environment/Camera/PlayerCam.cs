using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera effect used to follow the player
/// </summary>
public class PlayerCam : MonoBehaviour
{
    protected const float offsetY = 1.2f;
    public Transform player;
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
