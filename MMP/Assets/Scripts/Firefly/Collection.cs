using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    [SerializeField] GameObject player;
    private bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;
    public float xOffset;
    public float yOffset;
    public float dynamicOffsetVar = 3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {

            Vector3 initialOffset = new Vector3(xOffset, yOffset, 0);

            Vector3 dynamicOffset = initialOffset + new Vector3(Mathf.Sin(Time.time * dynamicOffsetVar), Mathf.Cos(Time.time * dynamicOffsetVar), 0);

            //Vector3 initialOffset = new Vector3(xOffset, yOffset, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.transform.position + dynamicOffset, ref vel, smoothTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            FireflyManager.instance.FireflyCollected();
        }
    }
}
