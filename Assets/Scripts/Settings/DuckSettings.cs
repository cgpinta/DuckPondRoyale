using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DuckBreed", menuName = "Duck Royale/New Duck Breed")]
public class DuckSettings : ScriptableObject
{
    [Header("Stats")] 
    [SerializeField] private float attack;
    public float Attack => attack;
    [SerializeField] private float defense;
    public float Defense => defense;
    [SerializeField] private float speed;
    public float Speed => speed;
    [SerializeField] private float swimSpeed;
    public float SwimSpeed => swimSpeed;
    [SerializeField] private float maxSpeed;
    public float MaxSpeed => maxSpeed;
    [SerializeField] private float jumpHeight;
    public float JumpHeight => jumpHeight;
    [SerializeField] private int flapCount;
    public int FlapCount => flapCount;
    [SerializeField] private float flapHeight;
    public float FlapHeight => flapHeight;

    [Header("Advanced Stats")]
    //[SerializeField] private float lowJumpMultiplier;
    //[SerializeField] private float fallMultiplier;
    [SerializeField] private float friction;
    public float Friction => friction;

    [Header("Cooldowns")]
    [SerializeField] private float flapCooldown;
    public float FlapCooldown => flapCooldown;
    [SerializeField] private float peckCooldown;
    public float PeckCooldown => peckCooldown;
    [SerializeField] private float honkCooldown;
    public float HonkCooldown => honkCooldown;
    [SerializeField] private float swimCooldown;
    public float SwimCooldown => swimCooldown;
    [SerializeField] private float throwCooldown;
    public float ThrowCooldown => throwCooldown;
    [SerializeField] private float landingCooldown;
    public float LandingCooldown => landingCooldown;

    [Header("Info")]
    [SerializeField] string breedName;
    public string BreedName => breedName;
    [TextArea]
    [SerializeField] string breedDescription;
    public string BreedDescription => breedDescription;

    [Header("Asthetics")]
    //[SerializeField] 
    GameObject duckObject;
    [SerializeField] Sprite inGameSprite;                       //used for SpriteRenderer
    [SerializeField] Sprite CSS;                                //used in CSS
}
