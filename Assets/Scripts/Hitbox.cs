using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum knockbackType
{
    Relative,                //The direction of knockback pushes away depending on the hitbox owner's direction  
    Fixed,                   //The direction of knockback pushes away in the same direction no matter the direction of the hitbox owner
    Centered                //The direction of knockback pushes away from the center of the hitbox
}
public enum hitboxShape
{
    Box,
    Circle
}
public class Hitbox : MonoBehaviour
{
    //[DisplayWithoutEdit()]
    public HitboxSettings settings;
    Attack attack;

    [SerializeField] bool editMode;
    [SerializeField] bool showGizmo;
    [SerializeField] bool active;

    [Header("Stats")]
    [SerializeField] public int priority;
    [SerializeField] float damage;
    [SerializeField] float knockback;
    [SerializeField] float hitstun;
    [SerializeField] Vector2 angle;
    [SerializeField] knockbackType type;

    [Header("Size")]
    [SerializeField] hitboxShape hitboxShapes;
    [SerializeField] CapsuleDirection2D capsuleDirection;
    [SerializeField] Vector2 center;
    [SerializeField] Vector2 size;
    [SerializeField] float circleRadius;
    [SerializeField] float hitboxAngle;

    Vector2 offset;
    Vector2 centerNoffset;


    private void Start()
    {
        hitboxShapes = settings.HitboxShapes;
        capsuleDirection = settings.CapsuleDirection;
        center = settings.Center;
        size = settings.Size;
        circleRadius = settings.CircleRadius;
        hitboxAngle = settings.HitboxAngle;
        attack = this.gameObject.GetComponent<Attack>();

        centerNoffset = center + offset;
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
            settings.setSettings(hitboxShapes, capsuleDirection, centerNoffset, size, circleRadius, hitboxAngle);
        }
        offset = attack.Offset;
        centerNoffset = center + offset;
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.red;
            drawHitbox();
        }
    }
    public void drawHitbox()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        switch (hitboxShapes)
        {
            case hitboxShape.Box:
                colliders = Physics2D.OverlapBoxAll(centerNoffset, size, hitboxAngle).ToList();
                Gizmos.DrawWireCube(centerNoffset, size);
                break;
            case hitboxShape.Circle:
                colliders = Physics2D.OverlapCircleAll(centerNoffset, circleRadius).ToList();
                Gizmos.DrawWireSphere(centerNoffset, circleRadius);
                break;
        }
        Debug.Log(colliders.ToString());
    }

    public List<Collider2D> renderHitbox()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        switch (hitboxShapes)
        {
            case hitboxShape.Box:
                colliders = Physics2D.OverlapBoxAll(centerNoffset, size, hitboxAngle).ToList();
                Gizmos.DrawWireCube(centerNoffset, size);
                break;
            case hitboxShape.Circle:
                colliders = Physics2D.OverlapCircleAll(centerNoffset, circleRadius).ToList();
                Gizmos.DrawWireSphere(centerNoffset, circleRadius);
                break;
        }

        return colliders.FindAll(x => x.gameObject.tag == "Hurtbox" && x.transform.root.gameObject.GetInstanceID() != gameObject.transform.root.gameObject.GetInstanceID()).ToList();
    }

    private List<Collider2D> removeDuplicateColliders(List<Collider2D> opposingColliders)
    {
        List<int> instanceIDs = new List<int>();
        foreach (Collider2D collider in opposingColliders)
        {
            if (!instanceIDs.Contains(collider.gameObject.transform.root.gameObject.GetInstanceID()))
            {
                instanceIDs.Add(collider.gameObject.transform.root.gameObject.GetInstanceID());
            }
            else
            {
                opposingColliders.Remove(collider);
            }
        }
        return opposingColliders;
    }

    private void Hit(Collider2D collider)
    {
        Debug.Log("HIT");
        float direction;
        Hurtbox hb = collider.GetComponent<Hurtbox>();
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
                Vector2 hypotenuse = this.gameObject.transform.root.position - collider.gameObject.transform.root.position;
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
