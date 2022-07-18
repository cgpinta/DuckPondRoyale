using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    Hittable hittable;

    // Start is called before the first frame update
    void Start()
    {
        hittable = this.transform.root.GetComponent<Hittable>();
    }

    public void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        hittable.GetHit(damage, knockback, hitstun, direction, type);
    }
}
