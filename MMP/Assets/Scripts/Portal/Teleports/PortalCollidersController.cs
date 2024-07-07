using UnityEngine;

public class PortalCollidersController : MonoBehaviour
{
    private GameObject player;
    public float verticalOffset = 0f;
    public float horizontalOffset = 0f;
    public float cubeSize = 22f;
    public float horizontalTeleportThreshold = 0.5f;
    public float topTeleportThreshold = 3.5f; // will probably be player height

    public GameObject topPortal;
    public GameObject bottomPortal;
    public GameObject leftPortal;
    public GameObject rightPortal;
    public BoxCollider2D topPortalCollider;
    public BoxCollider2D bottomPortalCollider;
    public BoxCollider2D leftPortalCollider;
    public BoxCollider2D rightPortalCollider;

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
        topPortal = CreateCollider("PortalTopCollider", PortalSide.Top);
        bottomPortal = CreateCollider("PortalBottomCollider", PortalSide.Bottom);
        leftPortal = CreateCollider("PortalLeftCollider", PortalSide.Left);
        rightPortal = CreateCollider("PortalRightCollider", PortalSide.Right);
    }

    GameObject CreateCollider(string name, PortalSide direction)
    {
        GameObject colliderObject = new GameObject(name);
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        PortalSideCollider sideCollider = colliderObject.AddComponent<PortalSideCollider>();
        sideCollider.Initialize(this, direction);
        colliderObject.transform.parent = transform;
        return colliderObject;
    }

    // void OnEnable()
    // {
    //     PositionColliders();
    // }

    void PositionColliders()
    {
        float halfSize = cubeSize / 2f;

        topPortal.transform.localPosition = new Vector3(0, halfSize + verticalOffset, 0);
        bottomPortal.transform.localPosition = new Vector3(0, -(halfSize + verticalOffset), 0);
        leftPortal.transform.localPosition = new Vector3(-(halfSize + horizontalOffset), 0, 0);
        rightPortal.transform.localPosition = new Vector3(halfSize + horizontalOffset, 0, 0);

        // Only here so it resets properly when cubeSize is changed in inspector
        topPortal.GetComponent<BoxCollider2D>().size = new Vector2(cubeSize, 2);
        bottomPortal.GetComponent<BoxCollider2D>().size = new Vector2(cubeSize, 2);
        leftPortal.GetComponent<BoxCollider2D>().size = new Vector2(2, cubeSize);
        rightPortal.GetComponent<BoxCollider2D>().size = new Vector2(2, cubeSize);

        topPortalCollider = topPortal.GetComponent<BoxCollider2D>();
        topPortalCollider.size = new Vector2(cubeSize, 2);

        bottomPortalCollider = bottomPortal.GetComponent<BoxCollider2D>();
        bottomPortalCollider.size = new Vector2(cubeSize, 2);

        leftPortalCollider = leftPortal.GetComponent<BoxCollider2D>();
        leftPortalCollider.size = new Vector2(2, cubeSize);

        rightPortalCollider = rightPortal.GetComponent<BoxCollider2D>();
        rightPortalCollider.size = new Vector2(2, cubeSize);
    }

    public void TeleportPlayer(PortalSide portalSide)
    {
        Vector3 playerPosition = player.transform.position;
        float playerHeight = player.GetComponent<Collider2D>().bounds.size.y;
        switch (portalSide)
        {
            case PortalSide.Bottom:
                float topPortalLowerBound = topPortalCollider.bounds.min.y;
                // Set the players new position so their lower bounds match the top portals lower bounds
                // half player offset because normally player center is on lower bound portal
                playerPosition.y = topPortalLowerBound + (playerHeight / 2);
                break;
            case PortalSide.Top:
                // Set the players new position so their lower bounds match the bottom portals upper bounds
                playerPosition.y = bottomPortalCollider.bounds.max.y + (playerHeight / 2);
                break;
            case PortalSide.Left:
                playerPosition.x = rightPortal.GetComponent<BoxCollider2D>().bounds.min.x;
                break;
            case PortalSide.Right:
                playerPosition.x = leftPortal.GetComponent<BoxCollider2D>().bounds.max.x;
                break;
        }

        player.transform.position = playerPosition;
        Debug.Log("Teleported player to: " + playerPosition);
    }

    void OnDrawGizmos()
    {
        float halfSize = cubeSize / 2f;

        // draw identical to portal colliders to see them in the UI
        Vector3 topLocalPosition = new Vector3(0, halfSize + verticalOffset, 0);
        Vector3 bottomLocalPosition = new Vector3(0, -(halfSize + verticalOffset), 0);
        Vector3 leftLocalPosition = new Vector3(-(halfSize + horizontalOffset), 0, 0);
        Vector3 rightLocalPosition = new Vector3(halfSize + horizontalOffset, 0, 0);

        Vector3 topPosition = transform.TransformPoint(topLocalPosition);
        Vector3 bottomPosition = transform.TransformPoint(bottomLocalPosition);
        Vector3 leftPosition = transform.TransformPoint(leftLocalPosition);
        Vector3 rightPosition = transform.TransformPoint(rightLocalPosition);

        Vector3 topBottomSize = new Vector3(cubeSize, 2);
        Vector3 leftRightSize = new Vector3(2, cubeSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(topPosition, topBottomSize);
        Gizmos.DrawWireCube(bottomPosition, topBottomSize);
        Gizmos.DrawWireCube(leftPosition, leftRightSize);
        Gizmos.DrawWireCube(rightPosition, leftRightSize);
    }
}

public enum PortalSide
{
    Bottom,
    Top,
    Left,
    Right
}

public class PortalSideCollider : MonoBehaviour
{
    private PortalCollidersController portalController;
    private PortalSide portalSide;

    public void Initialize(PortalCollidersController controller, PortalSide side)
    {
        portalController = controller;
        portalSide = side;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleTeleport(other);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleTeleport(other);
        }
    }

    void HandleTeleport(Collider2D other)
    {

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Bounds playerBounds = other.bounds;
        Bounds colliderBounds = GetComponent<Collider2D>().bounds;

        float minVelocityThreshold = 0f;

        // Debug.Log("v y: " + rb.velocity.y);
        // Debug.Log("v x: " + rb.velocity.x);
        // Debug.Log("Portalside :" + portalSide);


        switch (portalSide)
        {
            case PortalSide.Bottom:
                //if (/* rb.velocity.y > minVelocityThreshold &&*/ colliderBounds.max.y - playerBounds.min.y >= portalController.topTeleportThreshold)
                //{
                //    portalController.TeleportPlayer(portalSide);
                //}
                // if (rb.velocity.y > 0)
                // {
                //     rb.velocity = new Vector2(rb.velocity.x, -0.1f);
                // }
                if (rb.velocity.y < 0)
                {
                    portalController.TeleportPlayer(portalSide);
                }

                break;
            case PortalSide.Top:
                if (rb.velocity.y > minVelocityThreshold && playerBounds.min.y >= colliderBounds.min.y) //&& playerBounds.max.y - colliderBounds.min.y >= portalController.topTeleportThreshold)

                {
                    portalController.TeleportPlayer(portalSide);
                }
                break;
            case PortalSide.Left:
                if (rb.velocity.x < -minVelocityThreshold && colliderBounds.max.x - playerBounds.min.x >= (playerBounds.size.x / 2))
                {
                    portalController.TeleportPlayer(portalSide);
                }
                break;
            case PortalSide.Right:
                if (rb.velocity.x > minVelocityThreshold && playerBounds.max.x - colliderBounds.min.x >= (playerBounds.size.x / 2))
                {
                    portalController.TeleportPlayer(portalSide);
                }
                break;

        }
    }
}

