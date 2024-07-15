using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public float launchvelocity = 100f;

    // Update is called once per frame

    void Start() {
        gameObject.tag = "Player";
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Player";
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(10f, rb.velocity.y);
    }
    void Update()
    {
        //transform.position += launchvelocity * Time.deltaTime * -transform.right;
        Destroy(gameObject, 5f);   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy thisEnemy = collision.gameObject.GetComponent<Enemy>();
            if (thisEnemy != null)
            {
                thisEnemy.DamageEnemy(1, collision.gameObject);
            }
        }
        Destroy(gameObject);
    }
}
