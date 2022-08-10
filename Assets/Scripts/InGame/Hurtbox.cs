using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hurtbox : MonoBehaviour
{
    Hittable hittable;

    PlayerManager pManager;

    // Start is called before the first frame update
    void Start()
    {
        hittable = this.transform.root.GetComponent<Hittable>();

        pManager = FindObjectOfType<PlayerManager>();
    }

    public void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        hittable.GetHit(damage, knockback, hitstun, direction, type);
    }
    //private void OnTriggerEnter2d(Collider2D collider)
    //{
    //    Debug.Log("Player in death collider trigger");
    //    if (collider.tag == "Death")
    //    {
    //        Debug.Log("Player in death trigger");

    //        GameObject thisDuck = this.transform.root.gameObject;
    //        thisDuck.GetComponent<PlayerController>().Died();
    //        pManager.PlayerDied(thisDuck.GetComponent<PhotonView>().Owner, thisDuck.GetComponent<PlayerController>());
            
    //    }
    //}


}
