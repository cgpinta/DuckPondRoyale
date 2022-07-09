using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[RequireComponent(typeof)]
    //INITIALIZE OBJECTS
    [Header("Components")]
    public Transform tr;
    public Rigidbody2D rb;
    public CapsuleCollider2D cl;
    public Animator wingAnim;
    public Animator headAnim;
    public Animator bodyAnim;
    public Animator feetAnim;


    //INITIALIZE VARIABLES
    [Header("Movement")]
    public float speed;
    public float jumpSpeed;
    public int flapCount;
    public float flapHeight;
    public float friction;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Animation")]
    public float walkingAnimSpeed;

    [Header("Cooldowns")]
    public float flapCooldown = 2;
    public float peckCooldown = 2;


    //NON-PUBLIC VARIABLES

    private Vector2 movementVector;
    private bool inputJump, inputPeck;
    int currentFlaps;
    bool canControl, isDead;
    bool canPeck, canFlap;
    float anim_xvel; //xvelocity that is sent to the animations
    float anim_yvel;

    
    float flapCooldownTimer = 0;
    float peckCooldownTimer = 0;


    bool jumping, crouching, turning, isFalling, onGround;
    int againstWall;


    // START: is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<CapsuleCollider2D>();
        //animator = GetComponentInChildren<Animator>();

        //Debug.Log(bodyAnim.GetBool("isJumping"));

        //sprite.enabled = false;
        rb.simulated = true;
        canControl = true;
        isDead = false;
        onGround = false;
        canPeck = true;
    }



    private void Update()
    {
        Cooldowns();
    }

    // FIXED UPDATE: updates in delta time
    private void FixedUpdate()
    {

        NewMovementCode();

        StateAssignmentCode();

        AnimationVariables();
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
    }

    //whenever a state variable is set, it is set in here
    void StateAssignmentCode()
    {

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



    void NewMovementCode()
    {
        //Basic sideways movement
        if (againstWall != movementVector.x)
        {
            rb.velocity += Vector2.right * speed * movementVector.x *Time.deltaTime;
            //rb.velocity = new Vector2(movementVector.x * speed, rb.velocity.y);
            //rb.AddForce(new Vector2(movementVector.x * speed, rb.velocity.y));
        
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
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * Time.deltaTime);
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


    public bool getOnGround()
    {
        return onGround;
    }
    public void setOnGround(bool newValue)
    {
        onGround = newValue;
    }


    //INPUTS
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
            peckCooldownTimer = peckCooldown;
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
            rb.velocity = new Vector2(rb.velocity.x, flapHeight * Time.deltaTime);
            flapCooldownTimer = flapCooldown;
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
