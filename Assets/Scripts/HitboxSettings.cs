using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitboxSettings", menuName = "Duck Royale/New Hitbox Settings")]
public class Attack : ScriptableObject
{
    [SerializeField] float damage;
    [SerializeField] float knockback;
    [SerializeField] float hitstun;
    [SerializeField] Vector2 direction;
}
