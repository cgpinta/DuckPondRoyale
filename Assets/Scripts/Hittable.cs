using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Hittable : MonoBehaviour
{
    public enum knockbackType
    {
        Fixed,                  //The direction of knockback pushes away in the same direction no matter the direction of the hitbox owner
        Relative,               //The direction of knockback pushes away depending on the hitbox owner's direction
        Centered                //The direction of knockback pushes away from the center of the hitbox
    }

    public abstract void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type);

}
