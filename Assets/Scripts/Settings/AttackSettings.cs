using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSettings", menuName = "Duck Royale/Attack/New Attack Settings")]
public class AttackSettings : ScriptableObject
{
    [SerializeField] public Vector2 offset;
    public Vector2 Offset => offset;

    public void setSettings(Vector2 offset)
    {
        this.offset = offset;
        
    }
}
