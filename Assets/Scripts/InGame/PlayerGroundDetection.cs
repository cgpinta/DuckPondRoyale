using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour
{

    public PlayerController playerController;
    List<Collider2D> colliders = new List<Collider2D>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("entered");
        if (!collision.isTrigger)
        {
            colliders.Add(collision);
            //Debug.Log("GROUNDED");
            playerController.setOnGround(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (colliders.Count > 0)
        {
            playerController.setOnGround(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            colliders.Remove(collision);
            //Debug.Log("NOT GROUNDED");
            playerController.setOnGround(false);
        }
    }
}
