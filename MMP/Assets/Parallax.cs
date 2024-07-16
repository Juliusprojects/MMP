using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPosX;
    private float lengthX;
    public GameObject cam;
    public float parallaxEffectX;

    void Start()
    {
        startPosX = transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distX = (cam.transform.position.x - startPosX) * (1 - parallaxEffectX);

        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);

        if (distX > lengthX) startPosX += lengthX;
        else if (distX < -lengthX) startPosX -= lengthX;
    }
}
