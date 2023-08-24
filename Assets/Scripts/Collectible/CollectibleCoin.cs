using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectibleCoin
// manages the individual coins spawned in the world for the player to collect
// created 20/8/23
// last modified 24/8/23

public class CollectibleCoin : CollectibleBase
{
    [field: SerializeField] public int coinValue { get; private set; } = 1; // how many coins it is worth
}
