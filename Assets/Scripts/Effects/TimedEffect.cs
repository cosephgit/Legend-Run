using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TimedEffect
// for simple effects/sounds that should be instantiated and disappear when complete
// created 5/9/23
// last modified 5/9/23

public class TimedEffect : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private ParticleSystem[] particleSystems;

    private void Start()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }
        Destroy(gameObject, duration);
    }
}
