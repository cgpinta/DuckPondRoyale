using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : Hittable
{
    #region INITIALIZE OBJECTS
    [Header("Components")]
    public DuckSettings duckSettings;
    public Transform tr;
    public Rigidbody2D rb;
    public CapsuleCollider2D cl;
    public Animator wingAnim;
    public Animator headAnim;
    public Animator bodyAnim;
    public Animator feetAnim;

    PhotonView pView;
    #endregion

    #region VARIABLES
    [Header("Movement")]
    public float direction;
    public float speed;
    public float maxSpeed;
    public float jumpSpeed;
    public int flapCount;
    public float flapHeight;
    public float swimSpeed;
    public float friction;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Animation")]
    public float walkingAnimSpeed;

    [Header("Cooldowns")]
    public float flapCooldown;
    public float peckCooldown;
    public float crouchPeckCooldown;
    public float honkCooldown;
    public float crouchHonkCooldown;
    public float swimCooldown;
    public float landingCooldown;

    [Header("MoveDelay")]
    public float postSwimMoveDelay;


    [Header("Stats")]
    float damage;


    
    private Vector2 movementVector;
    private bool inputJump, inputPeck;
    int currentFlaps;
    bool canControl, isDead;
    bool canPeck, canFlap, canHonk, canSwim;
    float anim_xvel; //xvelocity that is sent to the animations
    float anim_yvel;

    float flapCooldownTimer;
    float peckCooldownTimer;
    float honkCooldownTimer;
    float swimCooldownTimer;
    float landingCooldownTimer;
    float hitstunCooldownTimer;

    bool jumping, crouching, turning, isFalling, onGround, inHitstun;
    int againstWall;

    float attack;
    float defense;
    #endregion

    // START: is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<CapsuleCollider2D>();

        pView = GetComponent<PhotonView>();

        rb.simulated = true;
        canControl = true;
        isDead = false;
        onGround = false;
        canPeck = true;
        inHitstun = false;

        damage = 0;
        speed = duckSettings.speed;
        maxSpeed = duckSettings.maxSpeed;
        jumpSpeed = duckSettings.jumpHeight;
        swimSpeed = duckSettings.swimSpeed;
        flapCount = duckSettings.flapCount;
        flapHeight = duckSettings.flapHeight;
        friction = duckSettings.friction;

        attack = duckSettings.attack;
        defense = duckSettings.defense;

        flapCooldown = duckSettings.flapCooldown;
        peckCooldown = duckSettings.peckCooldown;
        honkCooldown = duckSettings.honkCooldown;
        swimCooldown = duckSettings.swimCooldown;
        landingCooldown = duckSettings.landingCooldown;
    }

    private void Update()
    {
        Cooldowns();
    }

    // FIXED UPDATE: updates in delta time
    private void FixedUpdate()
    {
        if (pView.IsMine)
        {
            MovementCode();
            StateAssignmentCode();
            AnimationVariables();
        }
    }

    void Cooldowns()
    {
        if(flapCooldownTimer > 0)
        {
            //Debug.Log("flap timer: "+flapCooldownTimer);
            flapCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canFlap = true;
        }

        if(peckCooldownTimer > 0)
        {
            //Debug.Log("peck timer: " + peckCooldownTimer);
            peckCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canPeck = true;
        }

        if(honkCooldownTimer > 0)
        {
            honkCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canHonk = true;
        }

        if(swimCooldownTimer > 0)
        {
            swimCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canSwim = true;
        }
    }

    //whenever a state variable is set, it is set in here
    void StateAssignmentCode()
    {
        if(onGround && movementVector.y < 0)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        if (rb.velocity.y < 0)
            isFalling = true;
        else
            isFalling = false;


        if (onGround && jumping)
        {
            jumping = false;
        }

        if (onGround)
        {
            currentFlaps = flapCount;
        }
    }

    void MovementCode()
    {
        //Basic sideways movement
        if(onGround && (movementVector.x > 0 && rb.velocity.x < 0 || movementVector.x < 0 && rb.velocity.x > 0))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (onGround)
        {
            if(Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.velocity += Vector2.right * speed * movementVector.x * Time.deltaTime;
            }
        }
        else
        {
            rb.velocity += Vector2.right * maxSpeed * movementVector.x * Time.deltaTime;
        }

        if(Mathf.Abs(rb.velocity.x) < 0.1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }


        //if jump button pressed longer, player jumps higher
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !inputJump)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //jump
        if (inputJump && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }


        if(movementVector.x > 0)
        {
            direction = 1;
        }
        else if(movementVector.x < 0)
        {
            direction = -1;
        }

    }

    public void AnimationVariables()
    {
        //--------ANIMATION STUFF------------//

        if (rb.velocity.x > 0.01 || rb.velocity.x < -0.01)
            anim_xvel = rb.velocity.x;
        else
            anim_xvel = 0;

        if (rb.velocity.y > 0.01 || rb.velocity.y < -0.01)
            anim_yvel = rb.velocity.y;
        else
            anim_yvel = 0;

        anim_xvel = anim_xvel * walkingAnimSpeed;


        //Debug.Log(Mathf.Abs(anim_xvel));



        //SET ANIMATION PARAMETERS FOR BODY

        //bodyAnim.SetBool("isJumping", !onGround);
        //bodyAnim.SetBool("isTurning", turning);

        wingAnim.SetFloat("yVelocity", anim_yvel);
        headAnim.SetFloat("yVelocity", anim_yvel);
        bodyAnim.SetFloat("yVelocity", anim_yvel);
        feetAnim.SetFloat("yVelocity", anim_yvel);

        wingAnim.SetBool("isCrouching", crouching);
        headAnim.SetBool("isCrouching", crouching);
        bodyAnim.SetBool("isCrouching", crouching);
        //feetAnim.SetBool("isCrouching", crouching);

        //SET ANIMATION PARAMETERS FOR FEET
        if (onGround)
        {
            wingAnim.SetFloat("Speed", Mathf.Abs(anim_xvel));
            headAnim.SetFloat("Speed", Mathf.Abs(anim_xvel));
            bodyAnim.SetFloat("Speed", Mathf.Abs(anim_xvel));
            feetAnim.SetFloat("Speed", Mathf.Abs(anim_xvel));
        }
        else
        {
            wingAnim.SetFloat("Speed", 0);
            headAnim.SetFloat("Speed", 0);
            bodyAnim.SetFloat("Speed", 0);
            feetAnim.SetFloat("Speed", 0);
        }

                      


        //FLIP CHARACTER THE DIRECTION THEY MOVE
        if (anim_xvel > 0)
        {
            if (turning && !jumping)
                tr.rotation = Quaternion.Euler(tr.rotation.x, 0, tr.rotation.z);
            else
                tr.rotation = Quaternion.Euler(tr.rotation.x, 180, tr.rotation.z);
        }
        else if (anim_xvel < 0)
        {
            if (turning && !jumping)
                tr.rotation = Quaternion.Euler(tr.rotation.x, 180, tr.rotation.z);
            else
                tr.rotation = Quaternion.Euler(tr.rotation.x, 0, tr.rotation.z);
        }


    }

    #region ONGROUND METHODS
    public bool getOnGround()
    {
        return onGround;
    }
    public void setOnGround(bool newValue)
    {
        onGround = newValue;
    }
    #endregion

    #region INPUT METHODS
    public void Direction(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        inputJump = context.performed;
        Debug.Log("trying to jump"+onGround);
        if (context.performed && onGround)
        {
            jumping = true;
            Debug.Log("jumping");

        }
        else if (context.performed && !onGround && currentFlaps > 0)
        {
            Flap();
        }
    }

    public void Peck(InputAction.CallbackContext context)
    {
        if (canPeck)
        {
            canPeck = false;
            inputPeck = context.performed;
            wingAnim.SetTrigger("Peck");
            headAnim.SetTrigger("Peck");
            bodyAnim.SetTrigger("Peck");
            feetAnim.SetTrigger("Peck");
            if (crouching) { peckCooldownTimer = crouchPeckCooldown; }
            else { peckCooldownTimer = peckCooldown; }
        }
    }

    public void Flap()
    {
        if (canFlap)
        {
            canFlap = false;
            currentFlaps--;
            wingAnim.SetTrigger("Flap");
            headAnim.SetTrigger("Flap");
            bodyAnim.SetTrigger("Flap");
            feetAnim.SetTrigger("Flap");
            rb.velocity = new Vector2(rb.velocity.x, flapHeight);
            flapCooldownTimer = flapCooldown;
        }
    }
    public void Honk()
    {
        if (canHonk)
        {
            canHonk = false;
            currentFlaps--;
            wingAnim.SetTrigger("Honk");
            headAnim.SetTrigger("Honk");
            bodyAnim.SetTrigger("Honk");
            feetAnim.SetTrigger("Honk");
            if (crouching) { honkCooldownTimer = crouchHonkCooldown; }
            else { honkCooldownTimer = honkCooldown; }
        }
    }
    public void Swim()
    {
        if (canSwim)
        {
            canSwim = false;
            //wingAnim.SetTrigger("Swim");
            //headAnim.SetTrigger("Swim");
            //bodyAnim.SetTrigger("Swim");
            //feetAnim.SetTrigger("Swim");
            rb.velocity = movementVector * swimSpeed;
            swimCooldownTimer = swimCooldown;
        }
    }



    #endregion



    public override void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        this.damage += damage;
        if (hitstun > 0)
        {
            inHitstun = true;
            hitstunCooldownTimer = hitstun;
        }

        rb.velocity = direction * knockback * movementVector;

        //switch (type)
        //{
        //    case knockbackType.Fixed:
        //        rb.AddForce(direction * knockback);
        //        break;
        //    default:
        //        rb.velocity = (direction * knockback * movementVector);
        //        break;
        //}
    }


    private void OnTriggerEnter2d(Collider2D collider)
    {
        if (collider.CompareTag("Death"))
        {
            canControl = false;
            isDead = true;
            rb.velocity = Vector2.up * 100;
        }
    }
}
