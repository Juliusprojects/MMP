using System;
using UnityEngine;

[Obsolete]
public class PortalRightColliderController : MonoBehaviour
{
    private GameObject player; 
    public Collider2D leftCollider;
    public float offset = 1f;

    void Start()
    {
        GameObject leftPortal = GameObject.Find("PortalLeftCollider");
        if (leftPortal == null)
        {
            Debug.LogError("PortalLeftCollider GameObject not found!");
            return;
        }

        leftCollider = leftPortal.GetComponent<Collider2D>();
        if (leftCollider == null)
        {
            Debug.LogError("No Collider2D found on PortalLeftCollider GameObject!");
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
        if (other.gameObject == player && leftCollider != null)
        {
            Vector3 playerPosition = player.transform.position;
            playerPosition.x = leftCollider.transform.position.x + offset; // Apply offset towards the center to avoid being teleported back and forth
            player.transform.position = playerPosition;
            Debug.Log("Teleported player to: " + playerPosition);
        }
    }
}
