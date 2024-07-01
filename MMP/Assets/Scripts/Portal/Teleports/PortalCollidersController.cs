using UnityEngine;

public class PortalCollidersController : MonoBehaviour
{
    private GameObject player;
    public float verticalOffset = 0f;
    public float horizontalOffset = 0f;
    public float horizontalTeleportOffset = -1.6f;
    public float upwardTeleportOffset = 1f;
    public float downwardTeleportOffset = 1f;
    public float cubeSize = 22f;
    public float horizontalTeleportThreshold = 0.5f;
    public float topTeleportThreshold = 3.5f;
    public float bottomTeleportThreshold = 2.5f;

    public GameObject topCollider;
    public GameObject bottomCollider;
    public GameObject leftCollider;
    public GameObject rightCollider;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Please ensure the player has the 'Player' tag.");
            return;
        }

        InitializeColliders();
        PositionColliders();
    }

    void InitializeColliders()
    {
        topCollider = CreateCollider("PortalTopCollider", PortalDirection.Top);
        bottomCollider = CreateCollider("PortalBottomCollider", PortalDirection.Bottom);
        leftCollider = CreateCollider("PortalLeftCollider", PortalDirection.Left);
        rightCollider = CreateCollider("PortalRightCollider", PortalDirection.Right);
    }

    GameObject CreateCollider(string name, PortalDirection direction)
    {
        GameObject colliderObject = new GameObject(name);
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        PortalSideCollider sideCollider = colliderObject.AddComponent<PortalSideCollider>();
        sideCollider.Initialize(this, direction);
        colliderObject.transform.parent = transform;
        return colliderObject;
    }

    void OnEnable()
    {
        PositionColliders();
    }

    void PositionColliders()
    {
        float halfSize = cubeSize / 2f;

        topCollider.transform.localPosition = new Vector3(0, halfSize + verticalOffset, 0);
        bottomCollider.transform.localPosition = new Vector3(0, -(halfSize + verticalOffset), 0);
        leftCollider.transform.localPosition = new Vector3(-(halfSize + horizontalOffset), 0, 0);
        rightCollider.transform.localPosition = new Vector3(halfSize + horizontalOffset, 0, 0);

        topCollider.GetComponent<BoxCollider2D>().size = new Vector2(cubeSize, 2);
        bottomCollider.GetComponent<BoxCollider2D>().size = new Vector2(cubeSize, 2);
        leftCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, cubeSize);
        rightCollider.GetComponent<BoxCollider2D>().size = new Vector2(2, cubeSize);
    }

    public void TeleportPlayer(PortalDirection direction)
    {
        Vector3 playerPosition = player.transform.position;

        switch (direction)
        {
            case PortalDirection.Top:
                playerPosition.y = bottomCollider.transform.position.y + upwardTeleportOffset;
                break;
            case PortalDirection.Bottom:
                playerPosition.y = topCollider.transform.position.y - downwardTeleportOffset;
                break;
            case PortalDirection.Left:
                playerPosition.x = rightCollider.transform.position.x + horizontalTeleportOffset;
                break;
            case PortalDirection.Right:
                playerPosition.x = leftCollider.transform.position.x - horizontalTeleportOffset;
                break;
        }

        player.transform.position = playerPosition;
        Debug.Log("Teleported player to: " + playerPosition);
    }

    void OnDrawGizmos()
    {
        float halfSize = cubeSize / 2f;

        // Local Positions
        Vector3 topLocalPosition = new Vector3(0, halfSize + verticalOffset, 0);
        Vector3 bottomLocalPosition = new Vector3(0, -(halfSize + verticalOffset), 0);
        Vector3 leftLocalPosition = new Vector3(-(halfSize + horizontalOffset), 0, 0);
        Vector3 rightLocalPosition = new Vector3(halfSize + horizontalOffset, 0, 0);

        // Convert to World Positions
        Vector3 topPosition = transform.TransformPoint(topLocalPosition);
        Vector3 bottomPosition = transform.TransformPoint(bottomLocalPosition);
        Vector3 leftPosition = transform.TransformPoint(leftLocalPosition);
        Vector3 rightPosition = transform.TransformPoint(rightLocalPosition);

        // Sizes
        Vector3 topBottomSize = new Vector3(cubeSize, 2);
        Vector3 leftRightSize = new Vector3(2, cubeSize);

        // Draw the gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(topPosition, topBottomSize);
        Gizmos.DrawWireCube(bottomPosition, topBottomSize);
        Gizmos.DrawWireCube(leftPosition, leftRightSize);
        Gizmos.DrawWireCube(rightPosition, leftRightSize);
    }
}

public enum PortalDirection
{
    Top,
    Bottom,
    Left,
    Right
}

public class PortalSideCollider : MonoBehaviour
{
    private PortalCollidersController portalController;
    private PortalDirection direction;

    public void Initialize(PortalCollidersController controller, PortalDirection dir)
    {
        portalController = controller;
        direction = dir;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Bounds playerBounds = other.bounds;
            Bounds colliderBounds = GetComponent<Collider2D>().bounds;

            switch (direction)
            {
                case PortalDirection.Top:
                    if (playerBounds.max.y - colliderBounds.min.y >= portalController.topTeleportThreshold)
                    {
                        portalController.TeleportPlayer(direction);
                    }
                    break;
                case PortalDirection.Bottom:
                    if (colliderBounds.max.y - playerBounds.min.y >= portalController.bottomTeleportThreshold)
                    {
                        portalController.TeleportPlayer(direction);
                    }
                    break;
                case PortalDirection.Left:
                    if (colliderBounds.max.x - playerBounds.min.x >= portalController.horizontalTeleportThreshold)
                    {
                        portalController.TeleportPlayer(direction);
                    }
                    break;
                case PortalDirection.Right:
                    if (playerBounds.max.x - colliderBounds.min.x >= portalController.horizontalTeleportThreshold)
                    {
                        portalController.TeleportPlayer(direction);
                    }
                    break;
            }
        }
    }
}
