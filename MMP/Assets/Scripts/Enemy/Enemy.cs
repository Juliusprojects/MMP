using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int startPoint;
    private Rigidbody2D rb;
    public Transform[] points;
    public float speed;
    public int health = 5;
    private int i;

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
        transform.position = points[startPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length) 
            { 
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.SendMessage("ApplyDamage", 10);
            health--;
        }
        else if (collision.gameObject.tag == "PlayerProjectile")
        {
            health --;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
