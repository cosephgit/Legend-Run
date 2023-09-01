using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerWeapon
// manages the currently equipped player weapon
// created 24/8/23
// last modified 24/8/23

public class PlayerWeapon : MonoBehaviour
{
    private const string ANIM_ARMED = "Armed"; // keyword for the player being armed or not bool
    [SerializeField] private float weaponDiscardTime = 0.3f; // how long after being used up is the weapon discarded
    [SerializeField] private Transform weaponNode; // the node which the weapon should be attached to
    [SerializeField] private Animator pawnAnim;
    [SerializeField] private AudioClip[] attackSounds;
    public GameObject weapon { get; private set; } // the actual weapon, if there is one
    private float weaponDamage; // the currently equipped weapon strength
    private float weaponUses; // the currently equipped weapon health remaining

    private void DiscardWeapon()
    {
        // TODO maybe discard it later rather than just deleting it?
        Destroy(weapon);
        weapon = null;
        pawnAnim.SetBool(ANIM_ARMED, false);
    }

    // called by the PlayerPawn to use the weapon against an enemy encountere
    // returns the amount of enemy strength remaining after the weapon
    public float UseWeapon(float strength)
    {
        if (!weapon) return strength;

        if (strength <= weaponDamage && weaponUses > 0)
        {
            // TODO the actual weapon consumption stuff
            // the weapon needs to be discarded only AFTER the animation!!!
            // method 1: just discard after a timer (not very precise, but easy)
            // method 2: add animation keyframe (precise but may be fiddly and animation blending may skip it?)
            weaponUses = 0;
            Invoke("DiscardWeapon", weaponDiscardTime);

            if (attackSounds.Length > 0)
                AudioManager.instance.SoundPlayVaried(attackSounds[Random.Range(0, attackSounds.Length)], Vector2.zero);

            return 0f;
        }

        weaponUses = 0;
        Invoke("DiscardWeapon", weaponDiscardTime);

        return strength;
    }

    // called during animation to check if the weapon should be dropped (if it has been used up)
    public void CheckWeaponDrop()
    {
        if (weaponUses == 0) DiscardWeapon();
    }

    // ways to handle the weapon model placement? 
    // just grab it from the pickup and duplicate it?
    public void GetWeapon(CollectibleWeapon weaponNew)
    {
        if (weapon)
            DiscardWeapon();

        weapon = weaponNew.weaponObject;
        weapon.transform.parent = weaponNode;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        weaponDamage = weaponNew.weaponDamage;
        weaponUses = weaponNew.weaponUses;
        pawnAnim.SetBool(ANIM_ARMED, true);
    }
}
