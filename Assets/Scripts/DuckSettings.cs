using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DuckBreed", menuName = "Duck Royale/New Duck Breed")]
public class DuckSettings : ScriptableObject
{
    #region Public Variables
    [Header("Stats")]
    [SerializeField] public float health;
    [SerializeField] public float attack;
    [SerializeField] public float defense;
    [SerializeField] public float speed;
    [SerializeField] public float swimSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float jumpHeight;
    [SerializeField] public int flapCount;
    [SerializeField] public float flapHeight;

    [Header("Advanced Stats")]
    //[SerializeField] public float lowJumpMultiplier;
    //[SerializeField] public float fallMultiplier;
    [SerializeField] public float friction;

    [Header("Cooldowns")]
    [SerializeField] public float flapCooldown;
    [SerializeField] public float peckCooldown;
    [SerializeField] public float honkCooldown;
    [SerializeField] public float swimCooldown;
    [SerializeField] public float throwCooldown;
    [SerializeField] public float landingCooldown;

    [Header("Info")]
    [SerializeField] string breedName;
    [TextArea]
    [SerializeField] string breedDescription;

    [Header("Asthetics")]
    //[SerializeField] 
    GameObject duckObject;
    [SerializeField] Sprite inGameSprite;                       //used for SpriteRenderer
    [SerializeField] Sprite CSS;                                //used in CSS
    #endregion
}
