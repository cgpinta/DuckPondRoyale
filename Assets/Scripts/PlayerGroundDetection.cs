using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour
{

    public PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        //playerController = GetComponentInParent<PlayerController>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("entered");
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("GROUNDED");
            playerController.setOnGround(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !playerController.getOnGround())
        {
            Debug.Log("GROUNDED");
            playerController.setOnGround(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("NOT GROUNDED");
            playerController.setOnGround(false);
        }
    }
}
