using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public float launchvelocity = 13f;

    // Update is called once per frame

    void Start()
    {
        gameObject.tag = "Player";
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Player";
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        float v = transform.rotation.y >= 0 ? -launchvelocity : launchvelocity;
        rb.velocity = new Vector2(v, rb.velocity.y);
        Debug.Log($"VELOCITY: {v}");
    }
    void Update()
    {
        //transform.position += launchvelocity * Time.deltaTime * -transform.right;
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    void HandleCollision(Collider2D collider)
    {

        if (collider.gameObject.tag == "Enemy")
        {
            Enemy thisEnemy = collider.gameObject.GetComponent<Enemy>();
            if (thisEnemy != null)
            {
                thisEnemy.DamageEnemy(1, collider.gameObject);
            }
        }

        if (collider.isTrigger) return;

        Debug.Log($"COLLIDER NAME: {collider.gameObject} // ISTRIGGER: {collider.isTrigger}");
        Destroy(gameObject);
    }
}
