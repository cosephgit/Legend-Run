using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CollectibleWeapon
// collectible weapons
// created 24/8/23
// last modified 24/8/23

public class CollectibleWeapon : CollectibleBase
{
    [field: Header("Weapon")]
    [field: SerializeField] public int weaponUses { get; private set; } = 1; // how many times the weapon can be used before being lost
    [field: SerializeField] public float weaponDamage { get; private set; } = 1f; // how much damage the weapon inflicts
    [field: SerializeField] public GameObject weaponObject { get; private set; } // the actual object containing the weapon
}
