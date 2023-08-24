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
    [SerializeField] private AudioClip[] coinSound;

    public void Collected()
    {
        Destroy(gameObject);

        if (coinSound.Length > 0)
            AudioManager.instance.SoundPlayVaried(coinSound[Random.Range(0, coinSound.Length)], transform.position);
    }

    private void Update()
    {
        transform.Rotate(0f, degreesPerSec * Time.deltaTime, 0f);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
