using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]List<Hitbox> hitboxes;

    public AttackSettings settings;

    [SerializeField] bool editMode;
    [SerializeField] bool active;
    [Header("Properties")]
    [SerializeField] Vector2 offset;
    public Vector2 Offset => offset;



    private void Start()
    {
        offset = settings.offset;
        
    }

    private void Update()
    {
        if (active)
        {
            renderHitbox();
        }
    }

    private void OnValidate()
    {
        if (editMode)
        {
            if (settings == null) { return; }
            settings.setSettings(offset);
        }
    }

    private void drawHitbox()
    {
        foreach(Hitbox hitbox in hitboxes)
        {
            hitbox.drawHitbox();
        }
    }

    private Hitbox ComparePriority(List<Hitbox> hitboxes)
    {
        int highestPriority = -999;
        Hitbox highPrioHB = new Hitbox();
        foreach (Hitbox hitbox in hitboxes)
        {
            if(hitbox.priority > highestPriority)
            {
                highestPriority = hitbox.priority;
                highPrioHB = hitbox;
            }
        }
        return highPrioHB;
    }

    private void cleanupList(List<Collider2D> colliders)
    {
        for(int i=0; i < colliders.Count; i++)
        {

        }

        foreach (Hitbox hitbox in hitboxes)
        {
            List<Collider2D> currHitboxList = hitbox.renderHitbox();
            removeDuplicateColliders(currHitboxList);
            colliders.AddRange(currHitboxList);
        }
        removeDuplicateColliders(colliders);
        foreach (Collider2D collider in colliders)
        {
            Hit(collider);
        }
    }

    //private List<Collider2D> removeDuplicateColliders(List<Collider2D> opposingColliders)
    //{
    //    List<int> instanceIDs = new List<int>();
    //    foreach (Collider2D collider in opposingColliders)
    //    {
    //        if (!instanceIDs.Contains(collider.gameObject.transform.root.gameObject.GetInstanceID()))
    //        {
    //            instanceIDs.Add(collider.gameObject.transform.root.gameObject.GetInstanceID());
    //            Hit(collider);
    //        }
    //        else
    //        {
    //            opposingColliders.Remove(collider);
    //        }
    //    }
    //    return opposingColliders;
    //}

    //private void Hit(Collider2D collider)
    //{
    //    Debug.Log("HIT");
    //    float direction;
    //    Hurtbox hb = collider.GetComponent<Hurtbox>();
    //    Vector2 newAngle = new Vector2();

    //    switch (type)
    //    {
    //        case knockbackType.Relative:

    //            direction = this.gameObject.transform.root.GetComponent<PlayerController>().direction;
    //            Debug.Log(direction);
    //            newAngle = new Vector2(angle.x * direction, angle.y);
    //            break;
    //        case knockbackType.Centered:
    //            direction = this.gameObject.transform.root.GetComponent<PlayerController>().direction;
    //            Vector2 hypotenuse = this.gameObject.transform.root.position - collider.gameObject.transform.root.position;
    //            Vector2 horizontal = new Vector2(direction, 0);
    //            float angleDegree = Vector2.Angle(from: horizontal, to: hypotenuse);
    //            newAngle = (Vector2)(Quaternion.Euler(0, 0, angleDegree) * Vector2.right);

    //            break;
    //        default:
    //            newAngle = angle;
    //            break;
    //    }
    //    Debug.Log("Angle:" + newAngle + ", " + angle);
    //    hb.GetHit(damage, knockback, hitstun, newAngle, type);
    //}
}
