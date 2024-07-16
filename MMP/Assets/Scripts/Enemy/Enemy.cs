using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
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
    public int health = 1;
    private int i = 0;
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    private bool grounded = false;
    private int combinedGroundLayersMask;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //transform.position = points[startPoint].position;
        gameObject.GetComponent<Animator>().enabled = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        combinedGroundLayersMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Cave"));
    }

    // Update is called once per frame
    void Update()
    {
        //move towards the player
        if (points.Length == 0)
        {
            moveTowadsPlayer();
        }
        else 
        { 
            moveBetweenPoints();
        }
    }

    void moveTowadsPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) < 20f)
        {//move if distance from target is smaller than 20
         //rotate to look at the player
            if (transform.position.x - target.position.x <= 0)
            {
                gameObject.GetComponent<Animator>().SetTrigger("right");
            }
            else
            {
                gameObject.GetComponent<Animator>().SetTrigger("left");
            }
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            if (grounded == true)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            gameObject.GetComponent<Animator>().SetTrigger("stop");
            ChangeSprite(newSprite);
        }
    }

    void moveBetweenPoints()
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
        
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().deathParticle.Play();
            //deactivate Portal if Active
            var portal = GameObject.FindWithTag("Portal");
            if (portal != null)
            {
                portal.SetActive(false);
            }
            //respawnPlayer
            StartCoroutine(WaitAndRespawn(collision.gameObject));
        }
        else if ((combinedGroundLayersMask & (1 << collision.gameObject.layer)) != 0)
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

    IEnumerator WaitAndRespawn(GameObject player)
    {
        yield return new WaitForSeconds(0.3f);
        player.transform.position = player.GetComponent<PlayerController>().respawnPoint;
    }
}
