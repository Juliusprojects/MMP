using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;

//tesst
public class PlayerController : MonoBehaviour
{
    public float movePower = 10f;
    public float jumpPower = 25f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 5f;
    public float jumpCooldown = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private int direction = 1;
    private bool isJumping = false;
    private bool alive = true;

    // ground layer stuff
    private float groundedTime = 0f; // Time of impact when landing on ground layer
    private bool grounded;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("Ground");
        groundCheck = transform.Find("GroundCheck");
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // after changing running to a velocity based method to check velocity on portal collision the character moved laggy without this
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform not found. Please ensure there is a child object named GroundCheck");
        }
    }

    private void Update()
    {
        Restart();
        CheckGrounded();
        if (alive)
        {
            Hurt();
            Die();
            Attack();
            Jump();
            Run();
        }
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
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            // Reduce the upward velocity for a shorter jump when the jump key is released early
            rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }


    void Run()
    {
        Vector2 moveVelocity = Vector2.zero;
        anim.SetBool("isRun", false);

        if (InputUtil.Left())
        {
            direction = -1;
            moveVelocity = Vector2.left;
            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }
        else if (InputUtil.Right())
        {
            direction = 1;
            moveVelocity = Vector2.right;
            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }


        rb.velocity = new Vector2(moveVelocity.x * movePower, rb.velocity.y);
    }

    void Jump()
    {
        if (Time.time - groundedTime < jumpCooldown) return; // Check for jump cooldown

        if (InputUtil.Up() && !anim.GetBool("isJump") && grounded)
        {
            isJumping = true;
            anim.SetBool("isJump", true);
            rb.velocity = Vector2.up * jumpPower; // Jump
        }
        if (!isJumping)
        {
            return;
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wasGrounded = grounded;
        grounded = hit.collider != null; // Sets grounded to true if the ray hits the ground layer

        if (grounded && !wasGrounded) // Just landed
        {
            groundedTime = Time.time; // Set to time of impact when landing on ground layer
            isJumping = false; 
            anim.SetBool("isJump", false); 
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetTrigger("attack");
        }
    }

    void Hurt()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetTrigger("die");
            alive = false;
        }
    }

    void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim.SetTrigger("idle");
            alive = true;
        }
    }
}


