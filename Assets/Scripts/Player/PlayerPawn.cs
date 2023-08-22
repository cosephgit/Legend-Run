using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// takes player input and moves the player pawn depending on input
// created 18/8/23
// last modified 18/8/23

public class PlayerPawn : MonoBehaviour
{
    [SerializeField] private PlayerPawnHealth pawnHealth;
    [SerializeField] private PlayerPawnLoco pawnLoco;
    [SerializeField] private TerrainManager terrain;
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float speedMax = 20f;
    [SerializeField] private float speedAccel = 5f;
    [Header("Health settings")]
    [SerializeField] private float healthMax = 5f;
    [SerializeField] private UIHealthBar healthBar;
    [Header("Coin settings")]
    [SerializeField] private UICoinBar coinBar;
    private float pawnTargetX; // the pawn's current X target
    private bool pawnJump; // the pawn has been instructed to jump
    private float speed;
    private float health;
    private int coins;

    private void Awake()
    {
        pawnTargetX = transform.position.x;
        speed = 0;
        health = healthMax;
        healthBar.SetHealth(health / healthMax, true);
        coins = 0;
        coinBar.SetCoins(coins, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectibleCoin coin = other.gameObject.GetComponent<CollectibleCoin>();

        if (coin)
        {
            // have run into a coin, collect it!
            coin.Collected();
            coins++;
            coinBar.SetCoins(coins);
        }
        else
        {
            HazardBase hazard = other.gameObject.GetComponent<HazardBase>();

            if (hazard && hazard.awake)
            {
                // have run into a hazard!
                hazard.Collided();
                pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
                speed = 0f;
                TakeDamage(1f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0f, health - damage);
        healthBar.SetHealth(health / healthMax);
    }
    public void GainHealth(float heal)
    {
        health = Mathf.Min(health + heal, healthMax);
        healthBar.SetHealth(health / healthMax);
    }
    private void Update()
    {
        if (speed < speedMax)
        {
            // constantly increase the player's running speed
            speed = Mathf.Min(speed + speedAccel * Time.deltaTime, speedMax);
            pawnLoco.SetSpeed(speed / speedMax);
            terrain.SetTerrainSpeed(speed);
            // TODO: apply this to the terrain movement
        }

        // temp for testing
        pawnTargetX = Mathf.Clamp(transform.position.x + (Input.GetAxis("Horizontal") * pawnLoco.moveRate * Time.deltaTime), minX, maxX);
        pawnLoco.SetMoveTarget(pawnTargetX);

        if (Input.GetMouseButtonDown(0))
        {
            GainHealth(1f);
        }
    }
}
