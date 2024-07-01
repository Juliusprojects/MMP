using UnityEngine;

public class PortalTopColliderController : MonoBehaviour
{
    private GameObject player; 
    public Collider2D bottomCollider;
    public float offset = 4f; // Adjust this value as needed

    void Start()
    {
        GameObject bottomPortal = GameObject.Find("PortalBottomCollider");
        if (bottomPortal == null)
        {
            Debug.LogError("PortalBottomCollider GameObject not found!");
            return;
        }

        bottomCollider = bottomPortal.GetComponent<Collider2D>();
        if (bottomCollider == null)
        {
            Debug.LogError("No Collider2D found on PortalBottomCollider GameObject!");
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
        if (other.gameObject == player && bottomCollider != null)
        {
            Vector3 playerPosition = player.transform.position;
            playerPosition.y = bottomCollider.transform.position.y + offset; // Apply offset towards the center
            player.transform.position = playerPosition;
            Debug.Log("Teleported player to: " + playerPosition);
        }
    }
}
