using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : Hittable
{
    public string nickname;

    #region INITIALIZE OBJECTS
    [Header("Components")]
    public DuckSettings duckSettings;
    public Transform tr;
    public Rigidbody2D rb;
    public CapsuleCollider2D cl;

    AnimatorHolder anims;
    public Animator wingAnim;
    public Animator headAnim;
    public Animator bodyAnim;
    public Animator feetAnim;

    PhotonView pView;
    #endregion

    #region VARIABLES
    [Header("Movement")]
    public int direction;
    public float speed;
    public float maxSpeed;
    public float jumpSpeed;
    public int flapCount;
    public float flapHeight;
    public int swimCount;
    public float swimSpeed;
    public float friction;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Animation")]
    public float walkingAnimSpeed;

    [Header("Cooldowns")]
    public float flapLag;
    public float peckLag;
    public float crouchPeckLag;
    public float honkLag;
    public float crouchHonkLag;
    public float swimLag;
    public float shieldLag;
    public float aerialLag;
    public float landingLag;
    public float postSwimInvincibility;

    public float chargedPeckLag;
    public float chargedDirectionalPeckLag;
    public float chargedCrouchedPeckLag;
    public float chargedHonkLag;
    public float chargedCrouchHonkLag;
    


    [Header("Stats")]
    float damage;

    float swimInvincibility;

    public Vector2 movementVector;
    private bool inputJump, inputPeck;
    int currentFlaps;
    bool canControl, isDead;
    bool canPeck, canFlap, canHonk, canSwim;
    float anim_xvel; //xvelocity that is sent to the animations
    float anim_yvel;
    float turnAcceleration = 0.8f;

    //Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

    bool jumping, crouching, turning, swimming, isFalling, onGround, inHitstun, canMove;
    bool oldOnGround;
    bool pressingHonk;
    int againstWall;

    float attack;
    float defense;

    float attackChargeTime;
    float chargeThreshold = 0.2f;
    float maxChargeMultiplier = 4;
    float inputDeadzone = 0.6f;


    float frameLength = 1f/60;
    #endregion


    Timer flapTimer = new Timer();
    Timer peckTimer = new Timer();
    Timer honkTimer = new Timer();
    Timer swimTimer = new Timer();
    Timer shieldTimer = new Timer();
    Timer landingTimer = new Timer();
    Timer hitstunTimer = new Timer();
    Timer invincibleTimer = new Timer();
    Timer cantControlTimer = new Timer();
    Timer attackChargeTimer = new Timer(true);


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
        swimming = false;
        canSwim = true;
       
        damage = 0;
        speed = duckSettings.Speed;
        maxSpeed = duckSettings.MaxSpeed;
        jumpSpeed = duckSettings.JumpHeight;
        swimSpeed = duckSettings.SwimSpeed;
        flapCount = duckSettings.FlapCount;
        flapHeight = duckSettings.FlapHeight;
        friction = duckSettings.Friction;

        attack = duckSettings.Attack;
        defense = duckSettings.Defense;

        flapLag = duckSettings.FlapLag * frameLength;
        peckLag = duckSettings.PeckLag * frameLength;
        aerialLag = duckSettings.AerialLag * frameLength;
        honkLag = duckSettings.HonkLag * frameLength;
        swimLag = duckSettings.SwimLag * frameLength;
        landingLag = duckSettings.LandingLag * frameLength;

        chargedHonkLag = duckSettings.ChargedHonkLag * frameLength;

        Debug.Log("landing lag: "+duckSettings.LandingLag+" * "+frameLength+" = "+ landingLag);

        anims = new AnimatorHolder(wingAnim, headAnim, bodyAnim, feetAnim);

        direction = -1;
    }

    private void Update()
    {
        if (pView.IsMine)
        {
            StateAssignmentCode();
            AnimationVariables();
        }
    }


    // FIXED UPDATE: updates in delta time
    private void FixedUpdate()
    {
        if (pView.IsMine)
        {
            MovementCode();
        }

    }

    //whenever a state variable is set, it is set in here
    void StateAssignmentCode()
    {
        if(onGround && movementVector.y < -inputDeadzone)
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
        if (rb.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        if (onGround && jumping)
        {
            jumping = false;
        }
        if (onGround)
        {
            currentFlaps = flapCount;
            swimming = false;
            canSwim = true;
            if (swimTimer.isInProgress())
            {
                swimTimer.setTimer(0);
            }
        }
        swimming = swimTimer.isInProgress();
        
        if (!attackChargeTimer.isInProgress() && !landingTimer.isInProgress() && !hitstunTimer.isInProgress())
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }

    void MovementCode()
    {
        //Basic sideways movement
        if (onGround && (movementVector.x > 0 && rb.velocity.x < 0) || (movementVector.x < 0 && rb.velocity.x > 0))
        {
            if(Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            //else
            //{
            //    rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            //}
        }

        if (onGround)
        {
            if(Mathf.Abs(movementVector.x) > inputDeadzone)
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
        if (inputJump && onGround && canMove)
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


        //SET ANIMATION PARAMETERS FOR 
        anims.SetFloatForAll("yVelocity", anim_yvel);
        anims.SetBoolForAll("isCrouching", crouching);
        anims.SetBoolForAll("onGround", onGround);
        

        if(movementVector.y > inputDeadzone)
        {
            anims.SetFloatForAll("inputY", 1);
        }
        else if(movementVector.y < -inputDeadzone)
        {
            anims.SetFloatForAll("inputY", -1);
        }
        else
        {
            anims.SetFloatForAll("inputY", 0);
        }
            
        //SET ANIMATION PARAMETERS FOR FEET
        if (onGround)
        {
            anims.SetFloatForAll("Speed", Mathf.Abs(anim_xvel));
        }
        else
        {
            anims.SetFloatForAll("Speed", 0);
        }

            


        //FLIP CHARACTER THE DIRECTION THEY MOVE
        if(!peckTimer.isInProgress() && !honkTimer.isInProgress())
        {
            if (direction > 0)
            {
                if (turning && !jumping)
                    tr.rotation = Quaternion.Euler(tr.rotation.x, 0, tr.rotation.z);
                else
                    tr.rotation = Quaternion.Euler(tr.rotation.x, 180, tr.rotation.z);
            }
            else if (direction < 0)
            {
                if (turning && !jumping)
                    tr.rotation = Quaternion.Euler(tr.rotation.x, 180, tr.rotation.z);
                else
                    tr.rotation = Quaternion.Euler(tr.rotation.x, 0, tr.rotation.z);
            }
        }
    }

    #region ONGROUND METHODS
    public bool getOnGround()
    {
        return onGround;
    }
    public void setOnGround(bool newValue)
    {
        oldOnGround = onGround;
        if (newValue && !onGround)
        {
            landingTimer.setTimer(landingLag);
        }

        onGround = newValue;
    }
    #endregion

    #region INPUT METHODS
    private void Direction(InputAction.CallbackContext context)
    {
        if (!cantControlTimer.isInProgress())
        {
            movementVector = context.ReadValue<Vector2>();
        }
        
    }
    private void Jump(InputAction.CallbackContext context)
    {
        inputJump = context.performed;
        //Debug.Log("trying to jump"+onGround);
        if (context.performed && onGround)
        {
            jumping = true;
            //Debug.Log("jumping");

        }
        else if (context.performed && !onGround && currentFlaps > 0 && canMove)
        {
            Flap();
        }
    }

    private void Peck(InputAction.CallbackContext context)
    {
        if (!peckTimer.isInProgress() && !(attackChargeTimer.getType() != "peck" && attackChargeTimer.isInProgress()) && pView.IsMine)
        {
            if (context.started && !attackChargeTimer.isInProgress())
            {
                attackChargeTime = -1;
                anims.Head.SetBool("AttackCharging", true);
                attackChargeTimer.startWatch("peck");
            }
            if (context.canceled)
            {
                anims.Head.SetBool("AttackCharging", false);
                attackChargeTime = attackChargeTimer.stopWatch();
            }

            if (attackChargeTime >= 0)
            {
                PeckAttack(peckLag, crouchPeckLag, aerialLag);
            }
        }
    }

    public void PeckAttack(float lag, float crouchLag, float dirLag)
    {
        anims.SetTriggerForAll("Peck");
        if (crouching) { 
            peckTimer.setTimer(crouchLag); 
        }
        else if(movementVector.y > inputDeadzone || movementVector.y < -inputDeadzone){ 
            peckTimer.setTimer(dirLag); 
        }
        else
        {
            peckTimer.setTimer(lag);
        }
    }


    public void Honk(InputAction.CallbackContext context)
    {
        if (!honkTimer.isInProgress() && !(attackChargeTimer.getType() != "honk" && attackChargeTimer.isInProgress()) && pView.IsMine)
        {
            if (context.started && !attackChargeTimer.isInProgress())
            {
                attackChargeTime = -1;
                attackChargeTimer.startWatch("honk");

            }
            if (context.canceled)
            {
                anims.Head.SetBool("HonkCharging", false);
                attackChargeTime = attackChargeTimer.stopWatch();
            }

            if(attackChargeTimer.getWatch() > chargeThreshold)
            {
                anims.Head.SetBool("HonkCharging", true);
            }

            if (attackChargeTime >= 0f && attackChargeTime <= chargeThreshold)
            {
                HonkAttack(honkLag, crouchHonkLag, "Honk");
                anims.Head.SetBool("HonkCharging", false);
            }
            else if(attackChargeTime > chargeThreshold)
            {
                HonkAttack(chargedHonkLag, crouchHonkLag, "ChargedHonk");
            }
        }
    }

    public void HonkAttack(float lag, float crouchLag, string honkType)
    {
        anims.Head.SetTrigger(honkType);
        if (crouching) { 
            honkTimer.setTimer(crouchLag); 
        }
        else { 
            honkTimer.setTimer(lag);
        }
    }


    private void Flap()
    {
        if (!flapTimer.isInProgress() && pView.IsMine)
        {
            currentFlaps--;
            anims.SetTriggerForAll("Flap");
            rb.velocity = new Vector2(rb.velocity.x, flapHeight);
            flapTimer.setTimer(flapLag);
        }
    }
    
    private void Swim()
    {
        if (canSwim && pView.IsMine && canMove)
        {
            canSwim = false;
            anims.SetTriggerForAll("Swim");
            rb.velocity = new Vector2(direction, 0.5f) * swimSpeed;
            swimTimer.setTimer(swimLag);
            invincibleTimer.setTimer(postSwimInvincibility);
            cantControlTimer.setTimer(swimLag);
        }
    }
    #endregion

    [PunRPC]
    public override void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        if (pView.IsMine)
        {
            if (!invincibleTimer.isInProgress())
            {
                this.damage += damage;
                if (hitstun > 0)
                {
                    hitstunTimer.setTimer(hitstun);
                }
                else
                {
                    hitstunTimer.setTimer(0.5f);
                }

                //rb.velocity = direction.normalized * knockback * (this.damage / 5);
                rb.velocity = direction.normalized * knockback;
                Debug.Log(this.gameObject.name + " Player is hit: " 
                          + "Direction: " + direction.normalized 
                          + " Kb:" + knockback 
                          + " dmg/5: " + this.damage / 5 + " = " + rb.velocity);
                invincibleTimer.setTimer(0.01f);
            }
        }
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
