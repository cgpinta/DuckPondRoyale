using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class DeathDetection : MonoBehaviour
{
    PlayerManager pManager;
    GameObject thisDuck;
    Collider2D collider;

    bool deathQueued;

    Timer deathTimer = new Timer();

    // Start is called before the first frame update
    void Start()
    {
        deathQueued = false;
        pManager = transform.root.gameObject.GetComponent<PlayerController>().pManager;
        //pManager = FindObjectOfType<PlayerManager>();
        thisDuck = this.transform.root.gameObject;
        if(pManager != null)
        {
            pManager.PlayerRespawned += PlayerRespawn;
        }
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //if (deathQueued && !thisDuck.GetComponent<PlayerController>().isInvincible())
        if (deathQueued && !thisDuck.GetComponent<PlayerController>().isInvincible())
        {
            deathQueued=false;
            killPlayer();
        }
        if(pManager == null)
        {
            pManager = FindObjectOfType<PlayerManager>();
            if(pManager != null)
            {
                Debug.Log("pManager found");
                pManager.PlayerRespawned += PlayerRespawn;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Player in death collider trigger");
        //if (collision.tag == "Death" && !thisDuck.GetComponent<PlayerController>().isInvincible())
        if(collision.tag == "Death" && !deathTimer.isInProgress() && collision.transform.position != Vector3.zero)
        {
            Debug.Log("Player death from:"+collision.gameObject.name+" "+collision.transform.position);
            //Debug.Log();
            killPlayer();
            //if (!thisDuck.GetComponent<PlayerController>().isInvincible())
            //{
            //    killPlayer();
            //}
            //else
            //{
            //    deathQueued = true;
            //}
        }
    }


    void killPlayer()
    {
        Debug.Log("Kill player at:"+transform.position);
        thisDuck.transform.position = new Vector2(0, -1000);
        

        //thisDuck.GetComponent<PlayerController>().Died();
        collider.enabled = false;

        pManager.PlayerDied(thisDuck.GetComponent<PhotonView>().Owner, thisDuck.GetComponent<PlayerController>());
        //deathTimer.setTimer(0.05f);

    }

    void PlayerRespawn(Player player, PlayerController controller)
    {
        if (PhotonNetwork.IsConnected)
        {
            if (player == thisDuck.GetComponent<PhotonView>().Owner)
            {
                collider.enabled = true;
            }
        }
        else
        {
            if (controller.gameObject == this.transform.root.gameObject)
            {
                collider.enabled = true;
            }
        }
    }
}
