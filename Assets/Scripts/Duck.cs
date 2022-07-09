using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


[CreateAssetMenu(fileName = "DuckBreed", menuName = "Duck Royale/New Duck Breed")]
public class Duck : ScriptableObject
{

    [Header("Stats")]
    [SerializeField] int health;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;
    [SerializeField] int jumpHeight;
    [SerializeField] int flapCount;

    [Header("Advanced Stats")]
    [SerializeField] int lowJumpMultiplier;
    [SerializeField] int fallMultiplier;
    [SerializeField] int friction;



    [Header("Info")]
    [SerializeField] string breedName;
    [TextArea]
    [SerializeField] string breedDescription;

    [Header("Asthetics")]
    //[SerializeField] 
    GameObject duckObject;
    [SerializeField] AnimatorController inGameAnimator;         //used in-game
    [SerializeField] Sprite inGameSprite;                       //used for SpriteRenderer
    [SerializeField] Sprite CSS;                                //used in CSS

    [Header("Collider")]
    [SerializeField] Vector2 colliderOffset;
    [SerializeField] Vector2 colliderSize;
    [SerializeField] string colliderDirection;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loseHealth(int lostHealth)
    {
        health -= lostHealth;
    }

    public void healHealth(int healHealth)
    {
        health += healHealth;
    }

    public void takeKnockback(Vector2 direction, float magnitude)
    {

    }

    public void takeDamage(float damage)
    {

    }

    public void knockbackHit(Vector2 direction, float magnitude, float damage)
    {
        takeKnockback(direction, magnitude);
        takeDamage(damage);
    }
}
