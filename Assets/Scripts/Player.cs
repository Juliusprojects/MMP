using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private LayerMask playerMask;
    private bool jumping;
    private Rigidbody2D body;
    private float moveSpeed, dirX;

    private Vector2 normalJump = new Vector2(0, 5);
    

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        moveSpeed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        body.rotation = 0f;

        if(Input.GetKeyDown(KeyCode.Space)) {
            jumping = true;
            Debug.Log("space");
        } 
        dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
    }


    void FixedUpdate()
    {
        // JUMPING
        if (jumping) {
           if (Physics2D.OverlapCircleAll(groundCheck.position, 0.15f, playerMask).Length != 0) {
                body.AddForce(normalJump, ForceMode2D.Impulse);
                jumping = false;
           }
        }

        // WALKING
        body.velocity = new Vector2(dirX, body.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        
    }
    
}
