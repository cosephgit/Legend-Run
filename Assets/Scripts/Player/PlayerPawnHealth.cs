using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player health manager component
// created 18/8/23
// last modified 23/8/23

public class PlayerPawnHealth : MonoBehaviour
{
    [Header("Health settings")]
    [SerializeField] private float healthMax = 5f;
    [SerializeField] private UIHealthBar healthBar;
    [Header("Audio settings")]
    [SerializeField] private AudioClip[] painSounds;
    private float health;

    private void Awake()
    {
        health = healthMax;
        healthBar.SetHealth(health / healthMax, true);
    }

    public void TakeDamage(float damage, bool pain = true)
    {
        health = Mathf.Max(0f, health - damage);
        healthBar.SetHealth(health / healthMax);

        AudioManager.instance.SoundPlayVaried(painSounds[Random.Range(0, painSounds.Length)], Vector2.zero);
    }
    public void GainHealth(float heal)
    {
        health = Mathf.Min(health + heal, healthMax);
        healthBar.SetHealth(health / healthMax);
    }

    public bool IsAlive()
    {
        return (health > 0);
    }
}
