using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.2f;
    public Vector3 playerOffset = new Vector3(0, 5, -10);
    public Vector3 portalOffset = new Vector3(0, 0, -10);

    private Vector3 velocity = Vector3.zero;
    private GameObject portal;
    private bool isFollowing = true;

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

            if (isFollowing)
            {
                Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
    }
}
