using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// HazardBase
// this is the base class for all kinds of hazard which the player needs to avoid
// created 21/8/23
// last modified 7/9/23

public class HazardBase : MonoBehaviour
{
    [SerializeField] private Animator hazardAnim;
    [SerializeField] private AudioClip[] hazardSound;
    [SerializeField] private AudioClip[] hazardSoundDefeat;
    [SerializeField] private bool hazardSpin; // should the hazard be spun around randomly when placed (not for creatures with a front!)
    [field: SerializeField] public bool awake { get; private set; } = true;
    [field: SerializeField] public bool killable { get; private set; } = true;
    [field: SerializeField] public float hazardThreat = 1f; // for threat level calculations

    private void Awake()
    {
        if (hazardSpin)
        {
            transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        }
    }

    public void Collided()
    {
        if (awake)
        {
            if (hazardAnim)
            {
                if (CoSephUtils.RandomBool())
                    hazardAnim.SetTrigger("Attack3");
                else
                    hazardAnim.SetTrigger("Attack5");
            }

            if (hazardSound.Length > 0)
                AudioManager.instance.SoundPlayVaried(hazardSound[Random.Range(0, hazardSound.Length)], transform.position);

            awake = false;
        }
    }

    public void Defeated()
    {
        if (hazardAnim)
            hazardAnim.SetBool("Death", true);

        if (hazardSoundDefeat.Length > 0)
            AudioManager.instance.SoundPlayVaried(hazardSoundDefeat[Random.Range(0, hazardSoundDefeat.Length)], transform.position);

        awake = false;
    }
    public void Remove()
    {
        Destroy(gameObject);
    }
}
