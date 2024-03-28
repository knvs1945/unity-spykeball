using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for moving the game objects between walls
public class Portal : MonoBehaviour
{

    public float warpPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        Vector2 targetPos;
        if (collision.tag == "Player" || collision.tag == "Ball" || collision.tag == "Target" ) {
            Debug.Log("Warping...");
            targetPos = collision.GetComponent<Transform>().position;
            collision.GetComponent<Transform>().position = new Vector2(warpPoint, targetPos.y);
        }
    }
}
