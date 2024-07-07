using UnityEngine;

public class PortalManagerController : MonoBehaviour
{
    public GameObject portal; 
    private GameObject player; 

    public float xOffset = -0.3f; 
    public float yOffset = 2.3f; 

    public Vector3 spriteOffset = new Vector3(-0.4f, 0.18f, 0); 

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

        // Start with the portal deactivated
        try 
        {
            SetPortalActive(false);
        } 
        catch (UnassignedReferenceException e) 
        { 
            //if (!e.Message.Contains("Portal")) { throw e; };
        }
    }

    void Update()
    {
        // Toggle portal visibility with the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePortal();
        }
    }

    void TogglePortal()
    {

            if (portal.activeSelf)
            {
                SetPortalActive(false);
            }
            else
            {
                // Center the portal on the player before enabling it
                Vector3 offset = new Vector3(xOffset, yOffset, 0);
                portal.transform.position = player.transform.position + offset;
                ResetChildPositions();
                SetPortalActive(true);
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
}
