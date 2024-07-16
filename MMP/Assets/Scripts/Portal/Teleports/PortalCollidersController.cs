using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PortalCollidersController : MonoBehaviour
{
    private GameObject player;
    public float verticalOffset = 0f;
    public float horizontalOffset = 0f;
    public float cubeSize = 22f;
    // public float horizontalTeleportThreshold = 0.5f;
    // public float topTeleportThreshold = 3.5f; // will probably be player height

    public GameObject topPortal;
    public GameObject bottomPortal;
    public GameObject leftPortal;
    public GameObject rightPortal;
    public BoxCollider2D topPortalCollider;
    public BoxCollider2D bottomPortalCollider;
    public BoxCollider2D leftPortalCollider;
    public BoxCollider2D rightPortalCollider;

    private List<PortalSideCollider> portalSides = new();
    private List<BoxCollider2D> portalColliders = new();

    public GameObject PortalArea { get; private set; }
    public GameObject OuterPortalArea { get; private set; }
    private List<GameObject> extraBoxes = new();

    int combinedGroundLayersMask;


    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Please ensure the player has the 'Player' tag.");
            return;
        }
        //playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>().ToList();
        portalColliders = PortalIinit().Select(p => p.GetComponent<BoxCollider2D>()).ToList();
        PositionPortalColliders();

        ForceUpdateColliderBounds();
        PortalArea = PortalAreaInit();
        OuterPortalArea = OuterPortalAreaInit();

    }

    void Update()
    {
    }

    void OnEnable()
    {
        CreateOpposingColliders();
    }

    void Start()
    {

        //SetupMirrorDisplay(dMinX, dMaxX, sMinY, sMaxY, "left");
        //SetupMirrorDisplay(sMinX, sMaxX, sMinY, sMaxY);
        SetupCamerasAndDisplays();
        combinedGroundLayersMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Cave"));
    }



    void OnDisable()
    {
        foreach (var b in extraBoxes)
        {
            Destroy(b);
        }
    }

    #region mirrors



    public Vector2Int renderTextureSize = new Vector2Int(196, 1024); // Size of the Render Texture
    private List<Camera> mirrorCameras = new List<Camera>();
    //private RenderTexture mirrorRenderTexture;
    private List<GameObject> mirrorDisplays = new List<GameObject>();

    void SetupCamerasAndDisplays()
    {
        SetupLeftCameraAndDisplay();
        SetupRightCameraAndDisplay();
        SetupTopCameraAndDisplay();
    }

    private void SetupLeftCameraAndDisplay()
    {
        float sideOffset = 3f;
        var sMinX = leftPortalCollider.bounds.max.x - sideOffset;
        var sMaxX = leftPortalCollider.bounds.max.x;
        var sMaxY = topPortalCollider.bounds.min.y;
        var sMinY = bottomPortalCollider.bounds.max.y;

        var dMaxX = rightPortalCollider.bounds.min.x;
        var dMinX = rightPortalCollider.bounds.min.x - sideOffset;

        SetupMirrorCamera(sMinX, sMaxX, sMinY, sMaxY, dMinX, dMaxX, sMinY, sMaxY, "left");
        //SetupHidingCamera(sMinX, sMaxX, sMinY, sMaxY, "left");
    }

    private void SetupRightCameraAndDisplay()
    {
        float sideOffset = 3f;
        var sMinX = rightPortalCollider.bounds.min.x;
        var sMaxX = rightPortalCollider.bounds.min.x + sideOffset;
        var sMaxY = topPortalCollider.bounds.min.y;
        var sMinY = bottomPortalCollider.bounds.max.y;

        var dMaxX = leftPortalCollider.bounds.max.x + sideOffset;
        var dMinX = leftPortalCollider.bounds.max.x;

        SetupMirrorCamera(sMinX, sMaxX, sMinY, sMaxY, dMinX, dMaxX, sMinY, sMaxY, "right");
    }

    private void SetupTopCameraAndDisplay()
    {
        float sideOffset = 6f;
        var sMinX = topPortalCollider.bounds.min.x;
        var sMaxX = topPortalCollider.bounds.max.x;
        var sMaxY = topPortalCollider.bounds.min.y + sideOffset;
        var sMinY = topPortalCollider.bounds.min.y;

        var dMaxY = bottomPortalCollider.bounds.max.y + sideOffset;
        var dMinY = bottomPortalCollider.bounds.max.y;

        SetupMirrorCamera(sMinX, sMaxX, sMinY, sMaxY, sMinX, sMaxX, dMinY, dMaxY, "top");
    }

    public void SetupMirrorCamera(float minX, float maxX, float minY, float maxY, float dMinX, float dMaxX, float dMinY, float dMaxY, string suffix)
    {
        Vector3 position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, -10);
        float orthographicSize = (maxY - minY) / 2;

        RenderTexture mirrorRenderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 16);
        mirrorRenderTexture.name = "MirrorRenderTexture";
        GameObject mirrorCameraObject = new GameObject("MirrorCamera_" + suffix);
        mirrorCameraObject.transform.parent = transform;
        Camera mirrorCamera = mirrorCameraObject.AddComponent<Camera>();
        mirrorCamera.targetTexture = mirrorRenderTexture;
        mirrorCameraObject.transform.position = position;
        mirrorCamera.orthographic = true;
        mirrorCamera.orthographicSize = orthographicSize;
        mirrorCamera.aspect = (maxX - minX) / (maxY - minY);
        mirrorCamera.cullingMask = 1 << LayerMask.NameToLayer("Player");
        mirrorCamera.depth = -1;
        mirrorCameras.Add(mirrorCamera);


        // SpriteMask spriteMask = mirrorCameraObject.AddComponent<SpriteMask>();
        // spriteMask.sprite = Resources.Load<Sprite>("invisible.png");

        // SpriteRenderer[] playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>();
        // foreach (SpriteRenderer sr in playerSpriteRenderers)
        // {
        //     sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        // }

        //Debug.Log($"Camera Position: {mirrorCameraObject.transform.position}, Orth Size: {orthographicSize}, aspect: {mirrorCamera.aspect}");


        // DISPLAY

        Vector3 dPosition = new Vector3((dMinX + dMaxX) / 2, (dMinY + dMaxY) / 2, 1f);
        Vector2 size = new Vector2(dMaxX - dMinX, dMaxY - dMinY);

        GameObject mirrorDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mirrorDisplay.name = "MirrorDisplay_" + suffix;
        mirrorDisplays.Add(mirrorDisplay);
        // Temporarily unparent to set correct world space position and scale
        mirrorDisplay.transform.parent = null;
        mirrorDisplay.transform.position = dPosition;
        mirrorDisplay.transform.localScale = new Vector3(size.x, size.y, 0f);

        // Set the material and texture for the quad
        Material mirrorMaterial = new Material(Shader.Find("Unlit/Transparent"));
        mirrorMaterial.mainTexture = mirrorRenderTexture;
        mirrorMaterial.color = new Color(1, 1, 1, 0.5f);
        mirrorDisplay.GetComponent<Renderer>().material = mirrorMaterial;

        Renderer displayRenderer = mirrorDisplay.GetComponent<MeshRenderer>();
        displayRenderer.sortingLayerName = "Player";
        displayRenderer.sortingOrder = 0;

        mirrorDisplay.transform.parent = transform;
        mirrorDisplay.transform.localPosition = transform.InverseTransformPoint(dPosition);

        // Debug.Log($"sortingLayerName: {displayRenderer.sortingLayerName}, sortingOrder: {displayRenderer.sortingOrder}");
    }

    public void SetupHidingCamera(float minX, float maxX, float minY, float maxY, string suffix)
    {
        Vector3 position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        float orthographicSize = (maxY - minY) / 2;

        RenderTexture mirrorRenderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 16);
        mirrorRenderTexture.name = "MirrorRenderTexture";
        GameObject mirrorCameraObject = new GameObject("HidingCamera_" + suffix);
        mirrorCameraObject.transform.parent = transform;
        Camera mirrorCamera = mirrorCameraObject.AddComponent<Camera>();
        mirrorCamera.targetTexture = mirrorRenderTexture;
        mirrorCameraObject.transform.position = position;
        mirrorCamera.orthographic = true;
        mirrorCamera.orthographicSize = orthographicSize;
        mirrorCamera.aspect = (maxX - minX) / (maxY - minY);
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        mirrorCamera.cullingMask = ~(playerLayerMask);

        mirrorCamera.clearFlags = CameraClearFlags.SolidColor;
        mirrorCamera.backgroundColor = Color.black; // Optional: set a background color
        mirrorCamera.depth = 1;
        mirrorCameras.Add(mirrorCamera);
        mirrorCameras.Add(mirrorCamera);


        // SpriteMask spriteMask = mirrorCameraObject.AddComponent<SpriteMask>();
        // spriteMask.sprite = Resources.Load<Sprite>("invisible.png");

        // SpriteRenderer[] playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>();
        // foreach (SpriteRenderer sr in playerSpriteRenderers)
        // {
        //     sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        // }

        //Debug.Log($"Camera Position: {mirrorCameraObject.transform.position}, Orth Size: {orthographicSize}, aspect: {mirrorCamera.aspect}");


        // DISPLAY

        Vector3 dPosition = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 1f);
        Vector2 size = new Vector2(maxX - minX, maxY - minY);

        GameObject mirrorDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mirrorDisplay.name = "HidingDisplay_" + suffix;
        mirrorDisplays.Add(mirrorDisplay);
        // Temporarily unparent to set correct world space position and scale
        mirrorDisplay.transform.parent = null;
        mirrorDisplay.transform.position = dPosition;
        mirrorDisplay.transform.localScale = new Vector3(size.x, size.y, 0f);

        // Set the material and texture for the quad
        Material mirrorMaterial = new Material(Shader.Find("Unlit/Transparent"));
        mirrorMaterial.mainTexture = mirrorRenderTexture;
        mirrorMaterial.color = new Color(1, 1, 1, 0.5f);
        mirrorDisplay.GetComponent<Renderer>().material = mirrorMaterial;

        Renderer displayRenderer = mirrorDisplay.GetComponent<MeshRenderer>();
        displayRenderer.sortingLayerName = "Mirror2";
        displayRenderer.sortingOrder = 0;

        mirrorDisplay.transform.parent = transform;
        mirrorDisplay.transform.localPosition = transform.InverseTransformPoint(dPosition);

        // Debug.Log($"sortingLayerName: {displayRenderer.sortingLayerName}, sortingOrder: {displayRenderer.sortingOrder}");
    }






    // WORKING VERSION BUT BLACK CHARACTER ON SCREEN
    //  public void SetupMirrorDisplay(float minX, float maxX, float minY, float maxY)
    // {
    //     float width = maxX - minX;
    //     float height = maxY - minY;

    //     // Create the display GameObject and BoxCollider2D
    //     GameObject display = new GameObject("MirrorDisplay");
    //     display.transform.parent = transform;
    //     BoxCollider2D displayCollider = display.AddComponent<BoxCollider2D>();
    //     displayCollider.isTrigger = true;
    //     displayCollider.transform.position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2);
    //     displayCollider.size = new Vector2(width, height);

    //     // Create a quad to display the mirror camera's render texture
    //     GameObject displayQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //     displayQuad.transform.parent = display.transform;
    //     displayQuad.name = "MirrorDisplayQuad";
    //     //displayQuad.transform.SetParent(display.transform);

    //     // Set the quad's position and scale to match the BoxCollider2D
    //     displayQuad.transform.localPosition = Vector3.zero;
    //     displayQuad.transform.localScale = new Vector3(displayCollider.size.x, displayCollider.size.y, 1);

    //     // Set the material and texture for the quad
    //     MeshRenderer meshRenderer = displayQuad.GetComponent<MeshRenderer>();
    //     Material mirrorMaterial = new Material(Shader.Find("Unlit/Texture"));
    //     mirrorMaterial.mainTexture = renderTexture;
    //     meshRenderer.material = mirrorMaterial;

    //     // Disable shadows on the quad
    //     meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    //     meshRenderer.receiveShadows = false;
    //     //transform.SetParent(displayQuad.transform.parent);
    //     Debug.Log("Mirror display setup complete.");
    // }

    // public void SetupMirrorDisplay(float minX, float maxX, float minY, float maxY)
    // {
    //     float width = maxX - minX;
    //     float height = maxY - minY;

    //     // Create the display GameObject and BoxCollider2D
    //     GameObject display = new GameObject("MirrorDisplay");
    //     display.transform.parent = transform;
    //     BoxCollider2D displayCollider = display.AddComponent<BoxCollider2D>();
    //     displayCollider.isTrigger = true;
    //     displayCollider.transform.position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2);
    //     displayCollider.size = new Vector2(width, height);

    //     // Create a quad to display the mirror camera's render texture
    //     GameObject displayQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //     displayQuad.transform.parent = display.transform;
    //     displayQuad.name = "MirrorDisplayQuad";

    //     // Set the quad's position and scale to match the BoxCollider2D
    //     displayQuad.transform.localPosition = Vector3.zero;
    //     displayQuad.transform.localScale = new Vector3(displayCollider.size.x, displayCollider.size.y, 1);

    //     // Set the material and texture for the quad
    //     Material mirrorMaterial = new Material(Shader.Find("Unlit/Texture"));
    //     mirrorMaterial.mainTexture = mirrorRenderTexture;
    //     mirrorDisplay.GetComponent<Renderer>().material = mirrorMaterial;

    //     Debug.Log("Mirror display setup complete.");
    // }

    //     void SetupMirrorDisplay(float minX, float maxX, float minY, float maxY)
    //     {
    //         Vector3 position = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
    //         Vector2 size = new Vector2(maxX - minX, maxY - minY);
    //         GameObject mirrorDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //         mirrorDisplay.name = "MirrorDisplay";
    //         mirrorDisplay.transform.parent = transform;

    //         Material mirrorMaterial = new Material(Shader.Find("Unlit/Texture"));
    //         mirrorMaterial.mainTexture = mirrorRenderTexture;
    //         mirrorDisplay.GetComponent<Renderer>().material = mirrorMaterial;

    //         mirrorDisplay.transform.localPosition = transform.InverseTransformPoint(position);
    //         mirrorDisplay.transform.localScale = new Vector3(size.x, size.y, 1f);
    //         mirrorDisplay.transform.rotation = Quaternion.identity; // Ensure no rotation to avoid flipping

    //         // Adjust the mirror display's UVs to avoid flipping the image
    //         var meshFilter = mirrorDisplay.GetComponent<MeshFilter>();
    //         var mesh = meshFilter.mesh;
    //         Vector2[] uvs = new Vector2[4];
    //         uvs[0] = new Vector2(0, 0);
    //         uvs[1] = new Vector2(1, 0);
    //         uvs[2] = new Vector2(0, 1);
    //         uvs[3] = new Vector2(1, 1);
    //         mesh.uv = uvs;
    //     }



    #endregion

    #region opposingColliders
    public void CreateOpposingColliders()
    {
        List<Bounds> topPortalCollisionBounds = GetCollisionBoundsTopCollider();

        List<Bounds> bottomPortalCollisionBounds = GetCollisionBoundsBottomCollider();

        List<Bounds> leftPortalCollisionBounds = GetCollisionBoundsLeftCollider();

        List<Bounds> rightPortalCollisionBounds = GetCollisionBoundsRightCollider();

        foreach (Bounds b in topPortalCollisionBounds)
        {
            extraBoxes.Add(AddBoxCollider(b.min.x, b.max.x, bottomPortalCollider.bounds.min.y, bottomPortalCollider.bounds.max.y));
        }

        foreach (Bounds b in bottomPortalCollisionBounds)
        {
            extraBoxes.Add(AddBoxCollider(b.min.x, b.max.x, topPortalCollider.bounds.min.y, topPortalCollider.bounds.max.y));
        }

        foreach (Bounds b in leftPortalCollisionBounds)
        {
            extraBoxes.Add(AddBoxCollider(rightPortalCollider.bounds.min.x, rightPortalCollider.bounds.max.x, b.min.y, b.max.y));
        }

        foreach (Bounds b in rightPortalCollisionBounds)
        {
            extraBoxes.Add(AddBoxCollider(leftPortalCollider.bounds.min.x, leftPortalCollider.bounds.max.x, b.min.y, b.max.y));
        }
    }

    public GameObject AddBoxCollider(float minX, float maxX, float minY, float maxY)
    {
        GameObject colliderObject = new GameObject("ExtraBoxCollider");
        BoxCollider2D boxCollider = colliderObject.AddComponent<BoxCollider2D>();
        boxCollider.name = "extraBox";
        float width = maxX - minX;
        float height = maxY - minY;
        boxCollider.size = new Vector2(width, height);

        Vector2 center = new Vector2(minX + width / 2, minY + height / 2);
        colliderObject.transform.position = center;

        colliderObject.transform.parent = transform;

        return colliderObject;
    }


    private List<Bounds> GetCollisionBoundsTopCollider()
    {
        List<Bounds> collisionBounds = new List<Bounds>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(combinedGroundLayersMask);
        filter.useTriggers = false;

        List<Collider2D> results = new List<Collider2D>();

        int collisionCount = Physics2D.OverlapCollider(topPortalCollider, filter, results);
        //Debug.Log($"TEST: {collisionCount}");
        for (int i = 0; i < collisionCount; i++)
        {
            if (results[i].bounds.min.y > topPortalCollider.bounds.min.y) continue;
            Debug.Log("ADDED");
            collisionBounds.Add(results[i].bounds);
        }

        return collisionBounds;
    }

    private List<Bounds> GetCollisionBoundsBottomCollider()
    {
        List<Bounds> collisionBounds = new List<Bounds>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(combinedGroundLayersMask);
        filter.useTriggers = false;

        List<Collider2D> results = new List<Collider2D>();

        int collisionCount = Physics2D.OverlapCollider(bottomPortalCollider, filter, results);
        for (int i = 0; i < collisionCount; i++)
        {
            if (results[i].bounds.min.y < bottomPortalCollider.bounds.max.y) continue;
            collisionBounds.Add(results[i].bounds);
        }

        return collisionBounds;
    }

    private List<Bounds> GetCollisionBoundsLeftCollider()
    {
        List<Bounds> collisionBounds = new List<Bounds>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(combinedGroundLayersMask);
        filter.useTriggers = false;

        List<Collider2D> results = new List<Collider2D>();

        int collisionCount = Physics2D.OverlapCollider(leftPortalCollider, filter, results);
        for (int i = 0; i < collisionCount; i++)
        {
            if (results[i].bounds.max.x < leftPortalCollider.bounds.max.x) continue;
            collisionBounds.Add(results[i].bounds);
        }

        return collisionBounds;
    }

    private List<Bounds> GetCollisionBoundsRightCollider()
    {
        List<Bounds> collisionBounds = new List<Bounds>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ground"));
        filter.useTriggers = false;

        List<Collider2D> results = new List<Collider2D>();

        int collisionCount = Physics2D.OverlapCollider(rightPortalCollider, filter, results);
        for (int i = 0; i < collisionCount; i++)
        {
            if (results[i].bounds.max.x > rightPortalCollider.bounds.max.x) continue;
            collisionBounds.Add(results[i].bounds);
        }

        return collisionBounds;
    }


    #endregion

    #region portal colliders

    void ForceUpdateColliderBounds()
    {
        topPortalCollider.enabled = false;
        topPortalCollider.enabled = true;

        bottomPortalCollider.enabled = false;
        bottomPortalCollider.enabled = true;

        leftPortalCollider.enabled = false;
        leftPortalCollider.enabled = true;

        rightPortalCollider.enabled = false;
        rightPortalCollider.enabled = true;
    }

    private List<GameObject> PortalIinit()
    {
        topPortal = CreatePortalCollider("PortalTopCollider", PortalSide.Top);
        bottomPortal = CreatePortalCollider("PortalBottomCollider", PortalSide.Bottom);
        leftPortal = CreatePortalCollider("PortalLeftCollider", PortalSide.Left);
        rightPortal = CreatePortalCollider("PortalRightCollider", PortalSide.Right);

        topPortalCollider = topPortal.GetComponent<BoxCollider2D>();
        topPortalCollider.size = new Vector2(cubeSize, 2);

        bottomPortalCollider = bottomPortal.GetComponent<BoxCollider2D>();
        bottomPortalCollider.size = new Vector2(cubeSize, 2);

        leftPortalCollider = leftPortal.GetComponent<BoxCollider2D>();
        leftPortalCollider.size = new Vector2(2, cubeSize);

        rightPortalCollider = rightPortal.GetComponent<BoxCollider2D>();
        rightPortalCollider.size = new Vector2(2, cubeSize);

        return new List<GameObject>() { topPortal, bottomPortal, leftPortal, rightPortal };
    }

    GameObject CreatePortalCollider(string name, PortalSide direction)
    {
        GameObject colliderObject = new GameObject(name);
        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        PortalSideCollider sideCollider = colliderObject.AddComponent<PortalSideCollider>();
        sideCollider.Initialize(this, direction);
        portalSides.Add(sideCollider);
        colliderObject.transform.parent = transform;
        return colliderObject;
    }


    void PositionPortalColliders()
    {
        float halfSize = cubeSize / 2f;

        topPortal.transform.localPosition = new Vector3(0, halfSize + verticalOffset, 0);
        bottomPortal.transform.localPosition = new Vector3(0, -(halfSize + verticalOffset), 0);
        leftPortal.transform.localPosition = new Vector3(-(halfSize + horizontalOffset), 0, 0);
        rightPortal.transform.localPosition = new Vector3(halfSize + horizontalOffset, 0, 0);
    }


    #endregion

    #region portal areas


    GameObject PortalAreaInit()
    {
        float width = rightPortalCollider.bounds.min.x - leftPortalCollider.bounds.max.x;
        float height = topPortalCollider.bounds.min.y - bottomPortalCollider.bounds.max.y;
        GameObject portalArea = new GameObject("PortalArea");
        BoxCollider2D portalAreaCollider = portalArea.AddComponent<BoxCollider2D>();
        portalAreaCollider.size = new Vector2(width, height);
        portalAreaCollider.isTrigger = true;
        portalArea.transform.position = transform.position;
        portalArea.transform.parent = transform;
        return portalArea;
    }

    GameObject OuterPortalAreaInit()
    {
        float width = 2 * (rightPortalCollider.bounds.min.x - leftPortalCollider.bounds.max.x);
        float height = 2 * (topPortalCollider.bounds.min.y - bottomPortalCollider.bounds.max.y);
        GameObject portalArea = new GameObject("OuterPortalArea");
        BoxCollider2D portalAreaCollider = portalArea.AddComponent<BoxCollider2D>();
        portalAreaCollider.size = new Vector2(width, height);
        portalAreaCollider.isTrigger = true;
        portalArea.transform.position = transform.position;
        portalArea.transform.parent = transform;
        return portalArea;
    }
    #endregion

    #region teleport
    public void TeleportPlayer(PortalSide portalSide, Collider2D traveler)
    {
        Vector3 playerPosition = traveler.transform.position;
        switch (portalSide)
        {
            case PortalSide.Bottom:
                // Set the players new position so their lower bounds match the top portals lower bounds
                // half player offset because normally player center is on lower bound portal
                playerPosition.y = topPortalCollider.bounds.min.y;//+ (playerHeight / 2);
                break;
            case PortalSide.Top:
                playerPosition.y = bottomPortalCollider.bounds.max.y;
                break;
            case PortalSide.Left:
                playerPosition.x = rightPortalCollider.bounds.min.x;
                break;
            case PortalSide.Right:
                playerPosition.x = leftPortalCollider.bounds.max.x;
                break;
        }

        traveler.transform.position = playerPosition;
        Debug.Log("Teleported player to: " + playerPosition);
    }


    GameObject PlayerCloneInit()
    {
        var playerCollider = player.transform.GetComponent<Collider2D>();
        float width = playerCollider.bounds.size.x;
        float height = playerCollider.bounds.size.y;
        GameObject clone = new GameObject("clone");
        BoxCollider2D cloneCollider = clone.AddComponent<BoxCollider2D>();
        cloneCollider.size = new Vector2(width, height);
        cloneCollider.isTrigger = true;
        clone.transform.position = transform.position;
        return clone;
    }


    // public bool CanTeleport(PortalSide portalSide, Vector3 playerPosition)
    // {
    //     //return true;
    //     // Create a temporary GameObject with the same bounds as the player
    //     GameObject tempObject = PlayerCloneInit();
    //     Collider2D playerCollider = player.GetComponent<Collider2D>();
    //     BoxCollider2D tempCollider = tempObject.GetComponent<BoxCollider2D>();
    //     tempCollider.size = playerCollider.bounds.size;
    //     Vector3 newPosition = playerPosition;

    //     float playerHeight = playerCollider.bounds.size.y;
    //     switch (portalSide)
    //     {
    //         case PortalSide.Bottom:
    //             newPosition.y = topPortalCollider.bounds.min.y + (playerHeight / 2);
    //             break;
    //         case PortalSide.Top:
    //             newPosition.y = bottomPortalCollider.bounds.max.y + (playerHeight / 2);
    //             break;
    //         case PortalSide.Left:
    //             newPosition.x = rightPortalCollider.bounds.min.x;
    //             break;
    //         case PortalSide.Right:
    //             newPosition.x = leftPortalCollider.bounds.max.x;
    //             break;
    //     }

    //     // Move temporary object to the new position
    //     tempObject.transform.position = newPosition;

    //     //force load colliders
    //     tempCollider.enabled = false;
    //     tempCollider.enabled = true;

    //     // Debug.Log($"Temp object position: {tempObject.transform.position}");
    //     // Debug.Log($"Player object position: {playerPosition}");
    //     // Debug.Log($"Temp object size: {tempCollider.bounds.size}");
    //     // Debug.Log($"Player object size: {playerCollider.bounds.size}");

    //     // Check for collisions
    //     List<Collider2D> results = new List<Collider2D>();
    //     ContactFilter2D filter = new ContactFilter2D();
    //     filter.useTriggers = false;
    //     int collisionCount = tempObject.GetComponent<BoxCollider2D>().OverlapCollider(filter, results);
    //     //Debug.Log($"Number of collisions detected: {collisionCount}");
    //     Destroy(tempObject);
    //     List<Collider2D> results = new List<Collider2D>();
    //     return !overlaps;
    // }

    public bool CanTeleport(PortalSide portalSide, Vector3 playerPosition, Collider2D collider)
    {
        Vector3 newPosition = playerPosition;

        Vector2 areaStart = Vector2.zero;
        Vector2 areaEnd = Vector2.zero;

        float slack = 0.2f;
        switch (portalSide)
        {
            case PortalSide.Bottom:
                newPosition.y = topPortalCollider.bounds.min.y;
                areaStart = new Vector2(collider.bounds.min.x + 0.1f, newPosition.y + slack);
                areaEnd = new Vector2(collider.bounds.max.x - 0.1f, newPosition.y - slack);
                break;
            case PortalSide.Top:
                newPosition.y = bottomPortalCollider.bounds.max.y;
                areaStart = new Vector2(collider.bounds.min.x - 0.1f, newPosition.y);
                areaEnd = new Vector2(collider.bounds.max.x - 0.1f, newPosition.y);
                break;
            case PortalSide.Left:
                newPosition.x = rightPortalCollider.bounds.min.x;
                areaStart = new Vector2(newPosition.x, collider.bounds.min.y + 0.1f); // adding 0.1 so the ground on the other side doesnt block teleport
                areaEnd = new Vector2(newPosition.x, collider.bounds.max.y);
                break;
            case PortalSide.Right:
                newPosition.x = leftPortalCollider.bounds.max.x;
                areaStart = new Vector2(newPosition.x, collider.bounds.min.y + 0.1f);
                areaEnd = new Vector2(newPosition.x, collider.bounds.max.y);
                break;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;

        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapArea(areaStart, areaEnd, filter, results);



        List<Collider2D> realresults = results
            .Where(r => !portalColliders
                .Select(p => p.name)
                .Contains(r.name))
            .Where(r => r.name != "extraBox")
            .ToList();
        bool overlaps = realresults.Any();

        if (portalSide == PortalSide.Right)
        {
           // Debug.Log("Collisions: " + overlaps);
        }
        // Debug.Log($"New position: {newPosition}");
        // Debug.Log($"Area Start: {areaStart}, Area End: {areaEnd}");
        // Debug.Log($"Number of collisions detected: {results.Count}");
        // Debug.Log($"Number of actual collisions detected: {realresults.Count}");
        // Debug.Log($"names: {portalColliders.First().name}");


        switch (portalSide)
        {
            case PortalSide.Bottom:
                bottomPortalCollider.isTrigger = !overlaps || portalSide != PortalSide.Bottom;
                break;
            case PortalSide.Top:
                topPortalCollider.isTrigger = !overlaps || portalSide != PortalSide.Top;
                break;
            case PortalSide.Left:
                leftPortalCollider.isTrigger = !overlaps || portalSide != PortalSide.Left;
                break;
            case PortalSide.Right:
                rightPortalCollider.isTrigger = !overlaps || portalSide != PortalSide.Right;
                break;
        }


        return !overlaps;
    }

    public void ResetPortalTriggers()
    {
        foreach (var p in portalColliders)
        {
            p.isTrigger = true;
        }
    }
    #endregion

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
    public PortalSide portalSide;

    public void Initialize(PortalCollidersController controller, PortalSide side)
    {
        portalController = controller;
        portalSide = side;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var collider = other.GetComponent<Collider2D>();
            if (portalController.CanTeleport(portalSide, collider.transform.position, collider)) HandleTeleport(other);
        }
        portalController.ResetPortalTriggers();
    }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     Debug.Log("test");
    //     if (other.CompareTag("Player"))
    //     {
    //         portalController.ResetPortalTriggers();
    //     }
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleTeleport(other);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HandleTeleport(collision.collider);        
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HandleTeleport(collision.collider);
        }
    }

    public void Update()
    {
        portalController.topPortalCollider.isTrigger = true;
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.collider.CompareTag("Player"))
    //     {
    //         Collider2D playerCollider = collision.collider.GetComponent<Collider2D>();
    //         portalController.CanTeleport(portalSide, playerCollider.transform.position);
    //     }
    // }

    void HandleTeleport(Collider2D collider)
    {

        Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Bounds playerBounds = collider.bounds;
        Bounds colliderBounds = GetComponent<Collider2D>().bounds;
        float minVelocityThreshold = 0f;

        bool canTeleport = portalController.CanTeleport(portalSide, collider.transform.position, collider);
        Debug.Log($"CAN TELEPORT: {canTeleport}");
        if (!canTeleport) return;
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
                    portalController.TeleportPlayer(portalSide, collider);
                }

                break;
            case PortalSide.Top:
                if (rb.velocity.y > minVelocityThreshold && playerBounds.min.y >= colliderBounds.min.y) //&& playerBounds.max.y - colliderBounds.min.y >= portalController.topTeleportThreshold)

                {
                    portalController.TeleportPlayer(portalSide, collider);
                }
                break;
            case PortalSide.Left:
                if (rb.velocity.x < -minVelocityThreshold && colliderBounds.max.x - playerBounds.min.x >= (playerBounds.size.x / 2))
                {
                    portalController.TeleportPlayer(portalSide, collider);
                }
                break;
            case PortalSide.Right:
                if (rb.velocity.x > minVelocityThreshold && playerBounds.max.x - colliderBounds.min.x >= (playerBounds.size.x / 2))
                {
                    Debug.Log("TELEPORT TO LEFT");
                    portalController.TeleportPlayer(portalSide, collider);
                }
                break;

        }
    }
}
