using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public int startPoint;
    private Rigidbody2D rb;
    public Transform[] points;
    public Transform target;//set target from inspector instead of looking in Update
    public float speed = 3f;
    //public float speed;
    public int health = 5;
    private int i = 0; 

    // Start is called before the first frame update
    void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
        transform.position = points[startPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length) 
            { 
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        */

        //rotate to look at the player
        transform.LookAt(target.position);
        transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation

        
        //move towards the player
        if (Vector3.Distance(transform.position, target.position) > 0.03f && Vector3.Distance(transform.position, target.position) < 20f)
        {//move if distance from target is greater than 1
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //player dies and respawns
        }
    }

    public void DamageEnemy(int damageAmount, GameObject enemy)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(enemy);
        }
    }
}
