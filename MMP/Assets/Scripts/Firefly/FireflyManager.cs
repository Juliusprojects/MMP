using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyManager : MonoBehaviour
{
    public static FireflyManager instance;
    private int collectedFireflies = 0;
    public int totalFireflies = 3;

    void Start()
    {
       instance = this; 
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
        Debug.Log("Alle Glühwürmchen eingesammelt!");
    }
}
