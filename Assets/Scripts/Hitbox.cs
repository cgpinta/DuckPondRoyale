using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum knockbackType
{
    Relative,                //The direction of knockback pushes away depending on the hitbox owner's direction  
    Fixed,                   //The direction of knockback pushes away in the same direction no matter the direction of the hitbox owner
    Centered                //The direction of knockback pushes away from the center of the hitbox
}
public class Hitbox : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float knockback;
    [SerializeField] float hitstun;
    [SerializeField] Vector2 angle;
    [SerializeField] Hittable.knockbackType type;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HIT");
        if (collision.gameObject.tag == "Hurtbox" && collision.gameObject.transform.root != this.gameObject.transform.root)
        {
            Hurtbox hb = collision.GetComponent<Hurtbox>();
            Vector2 newAngle = new Vector2();

            switch (type)
            {
                case Hittable.knockbackType.Relative:
                    float direction = this.gameObject.transform.root.GetComponent<PlayerController>().direction;
                    newAngle = new Vector2(angle.x * direction, angle.y);
                    break;
                default:
                    newAngle = angle;
                    break;
            }
            Debug.Log("Angle:" + newAngle + ", " + angle);
            hb.GetHit(damage, knockback, hitstun, newAngle, type);
        }
    }
}
