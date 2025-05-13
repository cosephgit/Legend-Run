using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectibleBase
// base class for all collectible items
// created 24/8/23
// last modified 24/8/23

public class CollectibleBase : MonoBehaviour
{
    [Header("Collectible")]
    [SerializeField] private float degreesPerSec = 360f; // how quickly the coin rotates
    [SerializeField] private float bobAmplitude = 1f;
    [SerializeField] private float bobRate = 1f;
    [SerializeField] protected AudioClip[] pickupSound;
    public bool unused { get; protected set; } = true;

    public virtual void Collected()
    {
        unused = false;

        CollectSound();

        Destroy(gameObject);
    }

    public virtual void CollectSound()
    {
        if (pickupSound.Length > 0)
            AudioManager.instance.SoundPlayVaried(pickupSound[Random.Range(0, pickupSound.Length)], transform.position);
    }

    private void Update()
    {
        transform.Rotate(0f, degreesPerSec * Time.deltaTime, 0f);
    }

    public virtual void Remove()
    {
        unused = false;
        Destroy(gameObject);
    }
}
