using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// takes player input and moves the player pawn depending on input
// created 18/8/23
// last modified 18/8/23

public class PlayerPawn : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerPawnHealth pawnHealth;
    [SerializeField] private PlayerPawnLoco pawnLoco;
    [SerializeField] private PlayerPawnPurse pawnPurse;
    [SerializeField] private TerrainManager terrain;
    [Header("Movement parameters")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float speedMax = 20f;
    [SerializeField] private float speedAccel = 5f;
    [Header("Input parameters")]
    [SerializeField] private float screenActiveXMin = 0.2f; // left side main zone of screen
    [SerializeField] private float screenActiveXMax = 0.8f; // right side main zone of screen
    [SerializeField] private float screenJumpY = 0.5f; // minimum screen height to jump
    private float pawnTargetX; // the pawn's current X target
    private bool pawnJump; // the pawn has been instructed to jump
    private float speed;
    private bool jumpHeld;

    private void Awake()
    {
        pawnTargetX = transform.position.x;
        speed = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectibleCoin coin = other.gameObject.GetComponent<CollectibleCoin>();

        if (coin)
        {
            // have run into a coin, collect it!
            coin.Collected();
            pawnPurse.AddCoins(1);
        }
        else
        {
            HazardBase hazard = other.gameObject.GetComponent<HazardBase>();

            if (hazard && hazard.awake)
            {
                if (!pawnLoco.jumping)
                {
                    // have run into a hazard!
                    hazard.Collided();
                    pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
                    speed = 0f;
                    pawnHealth.TakeDamage(1f);
                }
            }
        }
    }

    // takes the screen position as input (from a touch or mouse) and converts it to a movement instruction
    private void SetMove(Vector2 touchPos)
    {
        // get the touch position in the main action area
        float touchX = (Mathf.Clamp(touchPos.x / Screen.width, screenActiveXMin, screenActiveXMax) - screenActiveXMin) / (screenActiveXMax - screenActiveXMin);
        float touchY = touchPos.y / Screen.height;

        // convert the touch inside the main area to a movement position
        pawnTargetX = minX + (maxX - minX) * touchX;
        pawnLoco.SetMoveTarget(pawnTargetX);

        if (touchY > screenJumpY)
        {
            // jump input received
            Debug.Log("Jump!");
            if (!jumpHeld)
            {
                jumpHeld = true;
                pawnLoco.StartJump();
            }
        }
        else
            jumpHeld = false;
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

        if (Input.touchCount > 0)
        {
            SetMove(Input.touches[0].position);
        }
        else if (Input.mousePresent && Input.GetMouseButton(0))
        {
            SetMove(Input.mousePosition);
        }
        else
        {
            // temp for testing
            pawnTargetX = Mathf.Clamp(transform.position.x + (Input.GetAxis("Horizontal") * pawnLoco.moveRate * Time.deltaTime), minX, maxX);
            pawnLoco.SetMoveTarget(pawnTargetX);

            if (Input.GetButton("Jump"))
            {
                if (!jumpHeld)
                {
                    jumpHeld = true;
                    pawnLoco.StartJump();
                }
            }
            else
            {
                jumpHeld = false;
            }
        }
    }
}
