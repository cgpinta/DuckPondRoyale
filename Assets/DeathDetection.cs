using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathDetection : MonoBehaviour
{
    PlayerManager pManager;
    GameObject thisDuck;
    Collider2D collider;

    bool deathQueued;

    // Start is called before the first frame update
    void Start()
    {
        deathQueued = false;
        pManager = FindObjectOfType<PlayerManager>();
        thisDuck = this.transform.root.gameObject;
        pManager.PlayerRespawned += PlayerRespawn;
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (deathQueued && !thisDuck.GetComponent<PlayerController>().isInvincible())
        {
            deathQueued=false;
            killPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Player in death collider trigger");
        if (collision.tag == "Death" && !thisDuck.GetComponent<PlayerController>().isInvincible())
        {
            Debug.Log("Player death");
            if (!thisDuck.GetComponent<PlayerController>().isInvincible())
            {
                killPlayer();
            }
            else
            {
                deathQueued = true;
            }
        }
    }


    void killPlayer()
    {
        thisDuck.transform.position = new Vector2(0, -1000);
        Debug.Log("Player in death trigger");

        //thisDuck.GetComponent<PlayerController>().Died();
        collider.enabled = false;
        pManager.PlayerDied(thisDuck.GetComponent<PhotonView>().Owner, thisDuck.GetComponent<PlayerController>());

    }

    void PlayerRespawn(Player player)
    {
        if(player == thisDuck.GetComponent<PhotonView>().Owner)
        {
            collider.enabled = true;
        }
    }
}
