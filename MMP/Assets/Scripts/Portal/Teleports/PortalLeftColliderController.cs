using System;
using UnityEngine;


[Obsolete]
public class PortalLeftColliderController : MonoBehaviour
{
    private GameObject player; 
    public Collider2D rightCollider;
    public float offset = 1f;

    void Start()
    {
        GameObject rightPortal = GameObject.Find("PortalRightCollider");
        if (rightPortal == null)
        {
            Debug.LogError("PortalRightCollider GameObject not found!");
            return;
        }

        rightCollider = rightPortal.GetComponent<Collider2D>();
        if (rightCollider == null)
        {
            Debug.LogError("No Collider2D found on PortalRightCollider GameObject!");
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
        if (other.gameObject == player && rightCollider != null)
        {
            Vector3 playerPosition = player.transform.position;
            playerPosition.x = rightCollider.transform.position.x - offset; // Apply offset towards the center to avoid being teleported back and forth
            player.transform.position = playerPosition;
            Debug.Log("Teleported player to: " + playerPosition);
        }
    }
}
