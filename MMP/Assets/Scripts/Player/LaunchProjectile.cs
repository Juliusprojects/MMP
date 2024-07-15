using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public float launchvelocity = 100f;

    // Update is called once per frame
    void Update()
    {
        transform.position += launchvelocity * Time.deltaTime * -transform.right;
        Destroy(gameObject, 5f);   
        gameObject.tag = "Projectile";
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