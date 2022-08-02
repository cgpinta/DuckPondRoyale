using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "DuckBreed", menuName = "Duck Royale/New Duck Breed")]
[Serializable]
public class DuckSettings : ScriptableObject
{
    [SerializeField] private int id;
    public int ID => id;
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
    [SerializeField] private float flapLag;
    [SerializeField] private float peckLag;
    [SerializeField] private float directionalPeckLag;
    [SerializeField] private float crouchPeckLag;
    [SerializeField] private float honkLag;
    [SerializeField] private float crouchHonkLag;
    [SerializeField] private float aerialLag;
    [SerializeField] private float swimLag;
    [SerializeField] private float throwLag;
    [SerializeField] private float sheildLag;
    [SerializeField] private float landingLag;


    [SerializeField] private float chargedPeckLag;
    [SerializeField] private float chargedDirectionalPeckLag;
    [SerializeField] private float chargedCrouchPeckLag;
    [SerializeField] private float chargedHonkLag;
    [SerializeField] private float chargedCrouchHonkLag;
    

    public float FlapLag => flapLag;
    public float PeckLag => peckLag;
    public float DirectionalPeckLag => directionalPeckLag;
    public float CrouchPeckLag => crouchPeckLag;
    public float HonkLag => honkLag;
    public float CrouchHonkLag => crouchHonkLag;
    public float AerialLag => aerialLag;
    public float SwimLag => swimLag;
    public float ThrowLag => throwLag;
    public float SheildLag => sheildLag;
    public float LandingLag => landingLag;


    public float ChargedPeckLag => chargedPeckLag;
    public float ChargedDirectionalPeckLag => chargedDirectionalPeckLag;
    public float ChargedCrouchPeckLag => chargedCrouchPeckLag;
    public float ChargedHonkLag => chargedHonkLag;
    public float ChargedCrouchHonkLag => chargedCrouchHonkLag;

    [Header("Info")]
    [SerializeField] string breedName;
    public string BreedName => breedName;
    [TextArea]
    [SerializeField] string breedDescription;
    public string BreedDescription => breedDescription;

    [Header("Asthetics")]
    //[SerializeField] 
    [SerializeField] GameObject duckObject;
    public GameObject DuckObject => duckObject;
    [SerializeField] Sprite css;                                //used in CSS
    public Sprite CSS => css;

}
