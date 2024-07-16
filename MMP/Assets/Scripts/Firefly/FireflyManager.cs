using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireflyManager : MonoBehaviour
{
    public GameObject Player;
    public static FireflyManager instance;
    private int collectedFireflies = 0;
    public int totalFireflies = 3;
    public float lightRadius = 20.0f; 
    public Color lightColor = new Color(255f / 255f, 223f / 255f, 70f / 255f, 1f);
    void Start()
    {
        instance = this;
        Player = GameObject.FindWithTag("Player");
    }

    public void FireflyCollected()
    {
        collectedFireflies++;
        if (collectedFireflies >= totalFireflies)
        {
            AllFirefliesCollected();
        }
    }

    private void AllFirefliesCollected()
    {
        Debug.Log("All fireflies collected!");

        Light2D light2D = Player.GetComponent<Light2D>();

        light2D.enabled = true;
        light2D.lightType = Light2D.LightType.Point;
        light2D.pointLightOuterRadius = lightRadius;
        light2D.intensity = 1.0f;
        light2D.color = lightColor; 
    }
}
