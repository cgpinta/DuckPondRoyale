using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float knockback;
    [SerializeField] float hitstun;
    [SerializeField] Vector2 direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //figure out how to figure out if a collider is connecting with an enemy

        Debug.Log("Hitbox: "+collision.gameObject.tag+", "+collision.gameObject.name);
    }
}
