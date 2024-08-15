using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectibleCoin
// manages the individual coins spawned in the world for the player to collect
// created 20/8/23
// last modified 24/8/23

public class CollectibleGem : CollectibleBase
{
    public void CollectedGem()
    {
        unused = false;

        if (pickupSound.Length > 0)
            AudioManager.instance.SoundPlayCustom(pickupSound[Random.Range(0, pickupSound.Length)], transform.position, 1f, 1f);

        Destroy(gameObject);
    }
}
