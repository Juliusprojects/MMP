using UnityEngine;

public class PortalManagerController : MonoBehaviour
{
    public GameObject portal; // Assign the Portal GameObject in the Inspector
    private GameObject player; // The player GameObject

    public float xOffset = -0.3f; // Offset for the x-axis
    public float yOffset = 2.3f; // Offset for the y-axis

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
        SetPortalActive(false);
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
        if (portal != null && player != null)
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
    }

    void SetPortalActive(bool isActive)
    {
        portal.SetActive(isActive);

        // Ensure all child objects are also set active or inactive
        //foreach (Transform child in portal.transform)
        //{
        //    child.gameObject.SetActive(isActive);
        //}
    }

    void ResetChildPositions()
    {
        foreach (Transform child in portal.transform)
        {
            // Reset the local position of each child to ensure correct positioning
            child.localPosition = Vector3.zero;
        }
    }
}
