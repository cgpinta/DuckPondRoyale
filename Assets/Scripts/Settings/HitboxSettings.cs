using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitboxSettings", menuName = "Duck Royale/New Hitbox Settings")]
public class HitboxSettings : ScriptableObject
{
    [SerializeField] private float damage;
    public float Damage => damage;
    [SerializeField] private float knockback;
    public float Knockback => knockback;
    [SerializeField] private float hitstun;
    public float Hitstun => hitstun;
    [SerializeField] private Vector2 angle;
    public Vector2 Angle => angle;
    [SerializeField] private knockbackType type;
    public knockbackType Type => type;

    [SerializeField] hitboxShape hitboxShapes;
    public hitboxShape HitboxShapes => hitboxShapes;
    [SerializeField] CapsuleDirection2D capsuleDirection;
    public CapsuleDirection2D CapsuleDirection => capsuleDirection;
    [SerializeField] Vector2 center;
    public Vector2 Center => center;
    [SerializeField] Vector2 size;
    public Vector2 Size => size;
    [SerializeField] float circleRadius;
    public float CircleRadius => circleRadius;
    [SerializeField] float hitboxAngle;
    public float HitboxAngle => hitboxAngle;

    public void setSettings(hitboxShape shape, CapsuleDirection2D capsuleDirection, Vector2 center, Vector2 size, float circleRadius, float hitboxAngle)
    {
        hitboxShapes = shape;
        this.capsuleDirection = capsuleDirection;
        this.center = center;
        this.size = size;
        this.circleRadius = circleRadius;
        this.hitboxAngle = hitboxAngle;
    }
}
