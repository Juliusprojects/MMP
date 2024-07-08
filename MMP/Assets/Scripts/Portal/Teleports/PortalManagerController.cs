using System.Collections.Generic;
using UnityEngine;
using Util;

public class PortalManagerController : MonoBehaviour
{
    public GameObject portal;
    private GameObject player;

    public float xOffset = -0.3f;
    public float yOffset = 2.3f;

    public Vector3 spriteOffset = new Vector3(-0.4f, 0.18f, 0);

    public GameObject portalArea;
    public GameObject outerPortalArea;


    PortalCollidersController PortalCollidersController;
    private List<Collider2D> disabledCollidersForPlayer = new List<Collider2D>();

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found!");
        }

        portal = GameObject.FindWithTag("Portal");
        if (portal == null)
        {
            Debug.LogError("Portal GameObject not found! Please ensure the portal has the 'Portal' tag.");
        }

        PortalCollidersController = FindObjectOfType<PortalCollidersController>();
        portalArea = PortalCollidersController.PortalArea;
        outerPortalArea = PortalCollidersController.OuterPortalArea;

        SetPortalActive(false); // Start with the portal deactivated
    }

    void Update()
    {
        if (InputUtil.Portal())
        {
            TogglePortal();
        }
    }

    void TogglePortal()
    {

        if (portal.activeSelf)
        {
            SetPortalActive(false);
            RestoreOriginalCollisions();
        }
        else
        {
            // Center the portal on the player before enabling it
            Vector3 offset = new Vector3(xOffset, yOffset, 0);
            portal.transform.position = player.transform.position + offset;
            ResetChildPositions();
            SetPortalActive(true);
            AdjustCollisions();
        }
    }

    private void SetPortalActive(bool isActive)
    {
        portal.SetActive(isActive);
    }

    void ResetChildPositions()
    {
        foreach (Transform child in portal.transform)
        {
            if (child.name == "PortalSprite")
            {
                // Apply the additional offset for the PortalSprite
                child.localPosition = spriteOffset;
            }
            else
            {
                // Reset the local position of other children to zero
                child.localPosition = Vector3.zero;
            }
        }
    }

    void AdjustCollisions()
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        List<Collider2D> outerColliders = new List<Collider2D>();
        List<Collider2D> portalColliders = new List<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D { useTriggers = false };

        // Get all colliders intersecting with outerPortalArea and portalArea
        outerPortalArea.GetComponent<BoxCollider2D>().OverlapCollider(filter, outerColliders);
        portalArea.GetComponent<BoxCollider2D>().OverlapCollider(filter, portalColliders);

        // Debug.Log(outerColliders.Count);
        // Debug.Log(portalColliders.Count);

        foreach (Collider2D c in outerColliders)
        {
            if (portalColliders.Contains(c)) continue;
            if ( c != playerCollider)
            {
                c.isTrigger = true;
                disabledCollidersForPlayer.Add(c);
            }
        }
    }



    void RestoreOriginalCollisions()
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        foreach (var c in disabledCollidersForPlayer)
        {
            if (c != null)
            {
                Physics2D.IgnoreCollision(playerCollider, c, false);
                c.isTrigger = false;
            }
        }
        disabledCollidersForPlayer.Clear();
    }
}
