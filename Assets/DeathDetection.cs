using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathDetection : MonoBehaviour
{
    PlayerManager pManager;
    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PlayerManager>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player in death collider trigger");
        if (collision.tag == "Death")
        {
            Debug.Log("Player in death trigger");

            GameObject thisDuck = this.transform.root.gameObject;
            //thisDuck.GetComponent<PlayerController>().Died();
            pManager.PlayerDied(thisDuck.GetComponent<PhotonView>().Owner, thisDuck.GetComponent<PlayerController>());

        }
    }
}
