using System;
using UnityEngine;

[Obsolete]
public class PortalBottomColliderController : MonoBehaviour
{
    private GameObject player; 
    public Collider2D topCollider;
    public float offset = 4f;

    void Start()
    {
        GameObject topPortal = GameObject.Find("PortalTopCollider");
        if (topPortal == null)
        {
            Debug.LogError("PortalTopCollider GameObject not found!");
            return;
        }

        topCollider = topPortal.GetComponent<Collider2D>();
        if (topCollider == null)
        {
            Debug.LogError("No Collider2D found on PortalTopCollider GameObject!");
            return;
        }

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Please ensure the player has the 'Player' tag.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && topCollider != null)
        {
            Vector3 playerPosition = player.transform.position;
            playerPosition.y = topCollider.transform.position.y - offset; // Apply offset towards the center to avoid being teleported back and forth
            player.transform.position = playerPosition;
            Debug.Log("Teleported player to: " + playerPosition);
        }
    }
}
