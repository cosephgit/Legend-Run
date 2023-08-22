using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectibleCoin
// manages the individual coins spawned in the world for the player to collect
// created 20/8/23
// last modified 20/8/23

public class CollectibleCoin : MonoBehaviour
{
    [SerializeField] private float degreesPerSec = 360f; // how quickly the coin rotates

    public void Collected()
    {
        Debug.Log("collected");
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(0f, degreesPerSec * Time.deltaTime, 0f);
    }
}
