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
    private float pawnTargetX; // the pawn's current X target
    private bool pawnJump; // the pawn has been instructed to jump
    private float speed;

    private void Awake()
    {
        pawnTargetX = transform.position.x;
        speed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectibleCoin coin = other.gameObject.GetComponent<CollectibleCoin>();

        if (coin)
        {
            // have run into a coin, collect it!
            coin.Collected();
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
            }
        }
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

    }
}
