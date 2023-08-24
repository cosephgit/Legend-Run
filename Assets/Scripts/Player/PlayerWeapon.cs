using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerWeapon
// manages the currently equipped player weapon
// created 24/8/23
// last modified 24/8/23

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform weaponNode; // the node which the weapon should be attached to
    [field: SerializeField] public GameObject weapon { get; private set; } // the actual weapon, if there is one
    private float weaponDamage; // the currently equipped weapon strength
    private float weaponUses; // the currently equipped weapon health remaining

    private void DiscardWeapon()
    {
        // TODO maybe discard it later?
        Destroy(weapon);
        weapon = null;
    }

    // called by the PlayerPawn to use the weapon against an enemy encountere
    // returns the amount of enemy strength remaining after the weapon
    public float UseWeapon(float strength)
    {
        if (!weapon) return strength;

        if (strength < weaponDamage)
        {
            // TODO the actual weapon consumption stuff
            DiscardWeapon();
            return 0f;
        }

        return strength;
    }

    // ways to handle the weapon model placement? 
    // just grab it from the pickup and duplicate it?
    public void GetWeapon(CollectibleWeapon weaponNew)
    {
        if (weapon)
            DiscardWeapon();

        Debug.Log("GetWeapon with " + weaponNew);

        weapon = weaponNew.weaponObject;
        weapon.transform.parent = weaponNode;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weaponDamage = weaponNew.weaponDamage;
        weaponUses = weaponNew.weaponUses;
    }
}
