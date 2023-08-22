using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HazardBase
// this is the base class for all kinds of hazard which the player needs to avoid
// created 21/8/23
// last modified 21/8/23

public class HazardBase : MonoBehaviour
{
    [SerializeField] private Animator hazardAnim;
    [field: SerializeField] public bool awake { get; private set; } = true;

    public void Collided()
    {
        if (awake)
        {
            Debug.Log("Bear go ROAR");
            if (CoSephUtils.RandomBool())
                hazardAnim.SetTrigger("Attack3");
            else
                hazardAnim.SetTrigger("Attack5");
            awake = false;
        }
    }
}
