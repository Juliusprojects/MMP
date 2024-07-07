using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Util;

//tesst
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpPower = 25f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 9f;
    public float jumpCooldown = 0.13f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isJumping = false;

    // ground layer stuff
    private float groundedTime = 0f; // Time of impact when landing on ground layer
    private bool grounded;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.Find("CharacterCrtl").GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("Ground");
        groundCheck = transform.Find("GroundCheck");
        // after changing running to a velocity based method to check velocity on portal collision the character moved laggy without this
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        // finally this fixed a bug where the player would overlap a bit with colliders when colliding on high velocity and triggering the portal if its close behind the wall
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform not found. Please ensure there is a child object named GroundCheck");
        }
    }

    private void Update()
    {
        CheckGrounded();
        if (InputUtil.Up()) { Jump(); }
        Move(InputUtil.HorizontalInput());
    }


    private void FixedUpdate()
    {
        ApplyBetterJumpPhysics();
    }

    private void ApplyBetterJumpPhysics()
    {
        if (rb.velocity.y < 0)
        {
            // Increase the fall speed by adding more downward force
            rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        else if (rb.velocity.y > 0 && !InputUtil.Up())
        {
            // Reduce the upward velocity for a shorter jump when the jump key is released early
            rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }



    void Move(float horizontalInput)
    {
        // move or set velocity to 0 when horizontalInput is 0)
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // animation
        if (horizontalInput != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(-horizontalInput), 1, 1); 
            if (!anim.GetBool("isJump2"))
            {                
                anim.SetBool("isRun2", true);             
            }
        }
        else
        {
            anim.SetBool("isRun2", false);            
        }

    }

    void Jump()
    {
        if (Time.time - groundedTime < jumpCooldown) return; // Check for jump cooldown

        if (!anim.GetBool("isJump2") && grounded)
        {
            //Debug.Log("Min height: " + minYPosition);
            isJumping = true;
            anim.SetTrigger("takeoff");
            anim.SetBool("isJump2", true);
            rb.velocity = Vector2.up * jumpPower; // Jump
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wasGrounded = grounded;
        grounded = hit.collider != null; // Sets grounded to true if the ray hits the ground layer

        if (grounded && !wasGrounded) // Just landed
        {
            //Debug.Log("Jump Height: " + (maxYPosition - transform.position.y));
            groundedTime = Time.time; // Set to time of impact when landing on ground layer
            isJumping = false;
            anim.SetBool("isJump2", false);

        }
    }
}


