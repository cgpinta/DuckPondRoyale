using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Hittable
{
    public Rigidbody2D rb;


    public override void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        //this.damage += damage;
        rb.AddForce(direction * knockback);
        Debug.Log("Cube: OW!");
    }
}
