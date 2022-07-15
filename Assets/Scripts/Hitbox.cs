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
    public HitboxSettings settings;
    [SerializeField] float damage;
    [SerializeField] float knockback;
    [SerializeField] float hitstun;
    [SerializeField] Vector2 angle;
    [SerializeField] knockbackType type;

    private void Start()
    {
        damage = settings.damage;
        knockback = settings.knockback;
        hitstun = settings.hitstun; 
        angle = settings.angle;
        type = settings.type;
    }

    private void Update()
    {
        if(settings == null) { return; }
        if (damage != settings.damage) { damage = settings.damage; }
        if (knockback != settings.knockback) { knockback = settings.knockback; }
        if (hitstun != settings.hitstun) { hitstun = settings.hitstun; }
        if (angle != settings.angle) { angle = settings.angle; }
        if (type != settings.type) { type = settings.type; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HIT");
        if (collision.gameObject.tag == "Hurtbox" && collision.gameObject.transform.root.gameObject.name != this.gameObject.transform.root.gameObject.name)
        {
            float direction;
            Hurtbox hb = collision.GetComponent<Hurtbox>();
            Vector2 newAngle = new Vector2();

            switch (type)
            {
                case knockbackType.Relative:
                    
                    direction = this.gameObject.transform.root.GetComponent<PlayerController>().direction;
                    Debug.Log(direction);
                    newAngle = new Vector2(angle.x * direction, angle.y);
                    break;
                case knockbackType.Centered:
                    direction = this.gameObject.transform.root.GetComponent<PlayerController>().direction;
                    Vector2 hypotenuse = this.gameObject.transform.root.position - collision.gameObject.transform.root.position;
                    Vector2 horizontal = new Vector2(direction, 0);
                    float angleDegree = Vector2.Angle(from: horizontal, to: hypotenuse);
                    newAngle = (Vector2)(Quaternion.Euler(0, 0, angleDegree) * Vector2.right);

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
