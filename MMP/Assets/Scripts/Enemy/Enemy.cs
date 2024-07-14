using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    //public int startPoint;
    //private Rigidbody2D rb;
    //public Transform[] points;
    public Transform target;//set target from inspector instead of looking in Update
    public float speed = 3f;
    //public float speed;
    public int health = 5;
    //private int i = 0;
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    private bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //transform.position = points[startPoint].position;
        gameObject.GetComponent<Animator>().enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
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


        //move towards the player
        if (Vector3.Distance(transform.position, target.position) < 10f)
        {//move if distance from target is smaller than 20
         //rotate to look at the player
            transform.LookAt(target.position);
            transform.Rotate(new Vector3(0, -90, 0), Space.Self);//correcting the original rotation
            gameObject.GetComponent<Animator>().enabled = true;
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        else
        {
            if (grounded == true) 
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            gameObject.GetComponent<Animator>().enabled = false;
            ChangeSprite(newSprite);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //player dies and respawns
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    public void DamageEnemy(int damageAmount, GameObject enemy)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(enemy);
        }
    }

    void ChangeSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }
}
