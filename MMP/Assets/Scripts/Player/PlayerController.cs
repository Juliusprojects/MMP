using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movePower = 10f;
    public float jumpPower = 25f; 
    public LayerMask groundLayer; 
    public Transform groundCheck; 
    public float groundCheckDistance = 0.1f; 

    public float fallMultiplier = 6f; 
    public float lowJumpMultiplier = 5f; 
    private Rigidbody2D rb;
    private Animator anim;
    private int direction = 1;
    bool isJumping = false;
    private bool alive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("Ground");
        groundCheck = transform.Find("GroundCheck");
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform not found. Please ensure there is a child object named GroundCheck");
        }
    }

    private void Update()
    {
        Restart();
        if (alive)
        {
            if (rb.velocity.y < 0) // Better jump feel
            {
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W))
            {
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            }

            Hurt();
            Die();
            Attack();
            Jump();
            Run();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        anim.SetBool("isJump", false);
    }

    void Run()
    {
        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            direction = -1;
            moveVelocity = Vector3.left;
            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            direction = 1;
            moveVelocity = Vector3.right;
            transform.localScale = new Vector3(direction, 1, 1);
            if (!anim.GetBool("isJump"))
                anim.SetBool("isRun", true);
        }
        transform.position += moveVelocity * movePower * Time.deltaTime;
    }

    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("Vertical") > 0) && !anim.GetBool("isJump") && IsGrounded())
        {
            isJumping = true;
            anim.SetBool("isJump", true);
            rb.velocity = Vector2.up * jumpPower; // Setting velocity directly for a consistent jump
        }
        if (!isJumping)
        {
            return;
        }
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null; // Returns true if the ray hits something on the ground layer
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