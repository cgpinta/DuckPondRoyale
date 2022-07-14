using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitboxSettings", menuName = "Duck Royale/New Hitbox Settings")]
public class HitboxSettings : ScriptableObject
{
    [SerializeField] public float damage;
    [SerializeField] public float knockback;
    [SerializeField] public float hitstun;
    [SerializeField] public Vector2 angle;
    [SerializeField] public knockbackType type;
}
