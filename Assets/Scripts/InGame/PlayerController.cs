using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using System;
using Unity.Burst.CompilerServices;

public class PlayerController : Hittable, IPunObservable
{
    public string nickname;

    #region INITIALIZE OBJECTS
    [Header("Components")]
    public DuckSettings duckSettings;
    Transform tr;
    Rigidbody2D rb;
    public CapsuleCollider2D footCollider;
    [SerializeField] GameObject sprite;
    
    [SerializeField] PlayerInput pInput;

    [Header("DO NOT CONFIGURE")]
    public DeathDetection deathDetect;
    public Player player;
    public PhotonView pView;
    public PlayerManager pManager;

    [Header("Animators")]
    AnimatorHolder anims;
    public Animator wingAnim;
    public Animator headAnim;
    public Animator bodyAnim;
    public Animator feetAnim;

    
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

    public PhysicsMaterial2D matFriction;
    public PhysicsMaterial2D matLowFriction;
    public PhysicsMaterial2D matNoFriction;
    public PhysicsMaterial2D matBouncy;

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
    //public float damage;
    public int lives;

    [Header("Other")]
    public float onGroundSlowDown;
    public float shortJumpMultiplier;
    public float airSpeedSlowdown;

    float swimInvincibility;

    public Vector2 movementVector;
    private bool inputJump, inputPeck, inputHonk;
    bool inAttack;
    bool jumpCancel;
    bool groundedSwim;

    int hitstunVerticalDirection;

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
    float inAttackGravityMultiplier = 0.5f;
    float hitstunSlowdownMultiplier = 0.1f;


    float frameLength = 1f/60;
    #endregion

    Timer flapTimer = new Timer();
    Timer peckTimer = new Timer();
    Timer honkTimer = new Timer();
    Timer swimTimer = new Timer();
    Timer shieldTimer = new Timer();
    Timer landingTimer = new Timer();
    Timer hitstunTimer = new Timer();
    Timer hitlagTimer = new Timer();
    Timer invincibleTimer = new Timer();
    Timer cantControlTimer = new Timer();
    Timer attackChargeTimer = new Timer(true);


    


    // START: is called before the first frame update
    void Awake()
    {
        anims = new AnimatorHolder(wingAnim, headAnim, bodyAnim, feetAnim);
        pManager = FindObjectOfType<PlayerManager>();
        if(pManager != null)
        {
            pManager.GameStart += ActivateInput;
            pManager.PlayerDied += Died;
        }

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        footCollider.sharedMaterial = matNoFriction;
        pView = GetComponent<PhotonView>();

        onGroundSlowDown = 0.955f;
        shortJumpMultiplier = .75f;
        airSpeedSlowdown = .5f;

        rb.simulated = true;
        canControl = true;
        isDead = false;
        onGround = false;
        canPeck = true;
        inHitstun = false;
        swimming = false;
        canSwim = true;
        inAttack = false;
        groundedSwim = false;

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

        

        direction = -1;

    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (pView.IsMine)
            {
                StateAssignmentCode();
                AnimationVariables();
            }
        }
        else
        {
            StateAssignmentCode();
            AnimationVariables();
        }
    }


    // FIXED UPDATE: updates in delta time
    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (pView.IsMine)
            {
                MovementCode();
            }
        }
        else
        {
            MovementCode();
        }
        //Debug.Log("position:"+tr.position);
    }

    //whenever a state variable is set, it is set in here
    void StateAssignmentCode()
    {
        if(onGround && movementVector.y < -inputDeadzone && !landingTimer.isInProgress())
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
            jumpCancel = false;
            swimming = false;
            //Debug.Log("Grounded swim:" + groundedSwim);
            if (!groundedSwim)
            {
                swimTimer.setTimer(0);
            }
            else
            {
                if (!swimTimer.isInProgress())
                {
                    groundedSwim = false;
                }
            }
        }
        canSwim = !swimTimer.isInProgress();
        
        if (!attackChargeTimer.isInProgress() && !landingTimer.isInProgress() && !hitstunTimer.isInProgress())
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }

        if (hitstunTimer.isInProgress())
        {
            if (rb.velocity.magnitude < 0.1)
            {
                hitstunTimer.setTimer(0);
            }
            if (onGround)
            {
                if (rb.velocity.magnitude < 5)
                {
                    hitstunTimer.setTimer(0);
                }
            }
            anims.SetBoolForAll("InHitstun", true);
            if(pInput != null)
                pInput.enabled = false;
            footCollider.sharedMaterial = matBouncy;
        }
        else
        {
            anims.SetBoolForAll("InHitstun", false);
            if (pInput != null)
                pInput.enabled = true;
            footCollider.sharedMaterial = matNoFriction;
        }

        
    }

    void MovementCode()
    {
        //Basic sideways movement
        if (hitlagTimer.isInProgress())
        {
            anims.SetBoolForAll("InHitstun", true);
            anims.DisableAll();
            rb.simulated = false;
            return;
        }
        else
        {
            if (anims.Head.enabled == false)
            {
                anims.EnableAll();
            }
            rb.simulated = true;
        }

        if (!hitstunTimer.isInProgress())
        {
            if (onGround)
            {
                if ((movementVector.x > 0 && rb.velocity.x < 0) || (movementVector.x < 0 && rb.velocity.x > 0))
                {
                    rb.velocity = Vector2.right * rb.velocity.x * -1 * Time.deltaTime;
                }

                if (Mathf.Abs(rb.velocity.x) < maxSpeed)
                {
                    if (Mathf.Abs(movementVector.x) > inputDeadzone)
                    {
                        rb.velocity += Vector2.right * speed * movementVector.x * Time.deltaTime;
                    }

                }
                if (!(Mathf.Abs(movementVector.x) > inputDeadzone))
                {
                    rb.velocity *= Vector2.right * onGroundSlowDown;
                }
            }
            else
            {
                rb.velocity += Vector2.right * (speed * airSpeedSlowdown) * movementVector.x * Time.deltaTime;
                if (swimming)
                {
                    if (Mathf.Abs(movementVector.x) > maxSpeed)
                    {
                        rb.velocity += Vector2.right * airSpeedSlowdown * 0.5f;
                    }
                }
            }
        }
        else
        {
            rb.velocity += Vector2.right * speed * hitstunSlowdownMultiplier * movementVector.x;
        }
        


        //if(Mathf.Abs(rb.velocity.x) < 0.1)
        //{
        //    rb.velocity = new Vector2(0, rb.velocity.y);
        //}
        if (Mathf.Abs(rb.velocity.y) < 0.1)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }


        float gravityMultipler = 1;
        if (inAttack)
        {
            gravityMultipler = inAttackGravityMultiplier;
        }

        //jump
        if (inputJump && onGround && canMove)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumping = false;
        }

        if (jumpCancel)
        {
            if(rb.velocity.y > jumpSpeed * shortJumpMultiplier)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * shortJumpMultiplier);
                jumpCancel = false;
            }
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
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.rotation.x, 0, sprite.transform.rotation.z);
                else
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.rotation.x, 180, sprite.transform.rotation.z);
            }
            else if (direction < 0)
            {
                if (turning && !jumping)
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.rotation.x, 180, sprite.transform.rotation.z);
                else
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.rotation.x, 0, sprite.transform.rotation.z);
            }
        }

        if (inputHonk && attackChargeTimer.getWatch() > chargeThreshold)
        {
            anims.Head.SetBool("HonkCharging", true);
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
        movementVector = context.ReadValue<Vector2>();   
    }
    private void Jump(InputAction.CallbackContext context)
    {
        inputJump = context.performed;
        //Debug.Log("trying to jump"+onGround);
        if (context.started && onGround)
        {
            jumping = true;

        }
        else if (context.performed && !onGround && currentFlaps > 0 && canMove)
        {
            Flap();
        }
        else if(context.canceled && !onGround)
        {
            jumpCancel = true;
        }
    }

    private void Peck(InputAction.CallbackContext context)
    {
        if (!peckTimer.isInProgress() && !(attackChargeTimer.getType() != "peck" && attackChargeTimer.isInProgress()) && pView.IsMine)
        {
            if (context.started && !attackChargeTimer.isInProgress())
            {
                inAttack = true;
                attackChargeTime = -1;
                anims.Head.SetBool("AttackCharging", true);
                attackChargeTimer.startWatch("peck");
            }
            if (context.canceled)
            {
                inAttack = false;
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
                inAttack = true;
                attackChargeTime = -1;
                attackChargeTimer.startWatch("honk");
                Debug.Log("honk input started");
                inputHonk = true;
            }


            if (context.canceled)
            {
                inAttack = false;
                anims.Head.SetBool("HonkCharging", false);
                attackChargeTime = attackChargeTimer.stopWatch();
                Debug.Log("honk input stopped");
                inputHonk = false;
            }

            
            Debug.Log("Honk Charging: "+anims.Head.GetBool("HonkCharging"));

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
    
    public void Swim(InputAction.CallbackContext context)
    {
        if (canSwim && pView.IsMine && canMove)
        {
            if (context.started)
            {
                canSwim = false;
                anims.SetTriggerForAll("Swim");
                if (Mathf.Abs(movementVector.y) > 0.5)
                {
                    int verticalDirection = (int)(Mathf.Abs(movementVector.y) / movementVector.y);
                    rb.velocity = new Vector2(direction, 0.5f * verticalDirection) * swimSpeed;
                }
                else
                {
                    rb.velocity = new Vector2(direction, 0.5f * movementVector.y) * swimSpeed;
                }

                if (!onGround)
                {
                    swimTimer.setTimer(swimLag);
                    cantControlTimer.setTimer(swimLag);
                }
                else
                {
                    swimTimer.setTimer(swimLag * .3f);
                    cantControlTimer.setTimer(swimLag * .3f);
                    groundedSwim = true;
                }
                swimming = true;
                invincibleTimer.setTimer(postSwimInvincibility);
            }
        }
    }

    public bool HUD = true;
    public void Defend(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            HUD = !HUD;
        }
    }



    #endregion



    public event Action UpdateDamage;
    [PunRPC]
    public override void GetHit(float damage, float knockback, float hitstun, Vector2 direction, knockbackType type)
    {
        if (pView.IsMine)
        {
            if (!invincibleTimer.isInProgress())
            {
                this.damage += damage;
                float calculatedHitstun = 1;
                if (hitstun > 0)
                {
                    calculatedHitstun = (this.damage + knockback + 1) * 0.04f;
                    hitstunTimer.setTimer(calculatedHitstun);
                }
                else
                {
                    hitstunTimer.setTimer(0.5f);
                }

                float calculatedHitlag = ((this.damage + knockback + 1) * (1f / 100f));
                hitlagTimer.setTimer(calculatedHitlag);

                //rb.velocity = direction.normalized * knockback * (this.damage / 5);
                rb.velocity = direction.normalized * knockback * ((this.damage + 1)/ 25);
                Debug.Log(this.gameObject.name + " Player is hit: " 
                          + "Direction: " + direction.normalized 
                          + " Kb:" + knockback 
                          + " dmg/5: " + this.damage / 5 + " = " + rb.velocity);
                invincibleTimer.setTimer(calculatedHitstun * 0.1f);
                
            }
        }
    }

    [PunRPC]
    public void Hit(float enemyCurDamage, float damage, float knockback)
    {
        float calculatedHitlag = (enemyCurDamage + damage + knockback + 1) * (1f / 100f);
        Debug.Log("enemyDamage:"+enemyCurDamage+" Damage:"+damage+" knockback:"+knockback+" Hitlag:"+calculatedHitlag);
        hitlagTimer.setTimer(calculatedHitlag);
    }

    private void ActivateInput()
    {
        this.GetComponent<PlayerInput>().ActivateInput();
    }

    private void Died(Player player, PlayerController controller)
    {
    } 

    //[PunRPC]
    public void Respawn(Vector3 spawnPoint)
    {
        invincibleTimer.setTimer(3);
        damage = 0;
        tr.position = spawnPoint;
        rb.velocity = Vector3.zero;
        Debug.Log("Player reset to:" + tr.position);
        isDead = false;
        
    }

    [PunRPC]
    public void LoseLife(Player player, PlayerController controller)
    {
        //if(player != this.player)
        //{
        //    return;
        //}
        lives--;
        //if(lives > 0)
        //{

        //}
    }

    public bool isInvincible()
    {
        return invincibleTimer.isInProgress();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(sprite.transform.rotation);
            stream.SendNext(damage);
            stream.SendNext(lives);
            stream.SendNext(player);
        }
        else
        {
            this.sprite.transform.rotation = (Quaternion)stream.ReceiveNext();
            this.damage = (float)stream.ReceiveNext();
            this.lives = (int)stream.ReceiveNext();
            this.player = (Player)stream.ReceiveNext();
        }
    }
}
