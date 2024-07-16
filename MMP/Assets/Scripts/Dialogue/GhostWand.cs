using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostWand : MonoBehaviour
{
    public static bool isCollected = false;


    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            isCollected = true;
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
