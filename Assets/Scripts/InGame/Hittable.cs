using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public abstract class Hittable : MonoBehaviour
{
    public float damage;

    [PunRPC]
    public abstract void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type);
}
