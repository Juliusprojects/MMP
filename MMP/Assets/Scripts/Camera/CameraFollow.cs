using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.2f;
    public Vector3 playerOffset = new Vector3(0, 4.3f, -15); // offset when focusing the player
    public Vector3 portalOffset = new Vector3(0.3f, 2, -15); // offset when portal is active

    private Vector3 velocity = Vector3.zero;
    private GameObject portal;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Vector3 initialPosition = player.position + playerOffset;
            transform.position = initialPosition;
        }

        portal = GameObject.FindWithTag("Portal");
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition;
            if (portal != null && portal.activeSelf)
            {
                targetPosition = portal.transform.position + portalOffset;
            }
            else
            {
                targetPosition = player.position + playerOffset;
            }

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
