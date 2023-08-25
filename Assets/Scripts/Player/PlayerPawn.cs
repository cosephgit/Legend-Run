using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// takes player input and moves the player pawn depending on input
// created 18/8/23
// last modified 25/8/23

public class PlayerPawn : MonoBehaviour
{
    public static PlayerPawn instance;
    [Header("Components")]
    [SerializeField] private PlayerPawnHealth pawnHealth;
    [SerializeField] private PlayerPawnLoco pawnLoco;
    [SerializeField] private PlayerPawnPurse pawnPurse;
    [SerializeField] private PlayerWeapon pawnWeapon;
    [SerializeField] private TerrainManager terrain;
    [SerializeField] private UIPointer pointer;
    [SerializeField] private UIStreak streakDisplay;
    [Header("Movement parameters")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float speedMax = 20f;
    [SerializeField] private float speedAccel = 5f;
    [Header("Input parameters")]
    [SerializeField] private float screenActiveXMin = 0.2f; // left side main zone of screen
    [SerializeField] private float screenActiveXMax = 0.8f; // right side main zone of screen
    [SerializeField] private float screenJumpY = 0.5f; // minimum screen height to jump
    [Header("Streak params")]
    [SerializeField] private int streakCountBase = 5;
    [SerializeField] private int streakCountPerLevel = 5;
    [SerializeField] private AudioClip streakLevelSound;
    [SerializeField] private AudioClip streakFailSound;
    [SerializeField] private float streakCoinPitchBase = 1f;
    [SerializeField] private float streakCoinPitchPerLevel = 0.05f;
    [SerializeField] private float streakCoinPitchForProgress = 0.2f;
    private float pawnTargetX; // the pawn's current X target
    private bool pawnJump; // the pawn has been instructed to jump
    private float speed;
    private bool jumpHeld;
    // streak values
    private int streakCoins;
    private int streakLevel;

    private void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
            instance = this;

        pawnTargetX = transform.position.x;
        speed = 0;
    }

    public void StreakEnd()
    {
        if (streakLevel > 0)
        {
            streakDisplay.StreakBreak();
            AudioManager.instance.SoundPlayEven(streakFailSound, Vector2.zero);
        }
        streakLevel = 0;
        streakCoins = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectibleCoin coin = other.gameObject.GetComponent<CollectibleCoin>();

        if (coin)
        {
            if (coin.unused)
            {
                int streakCoinsLevel = streakCountBase * streakLevel + streakCountPerLevel * (streakLevel - 1) * (streakLevel) / 2; // number of coins to the start of this level
                int streakCoinsLevelNext = streakCountBase + streakCountPerLevel * streakLevel; // number of coins from the start of this level to the next level
                float pitch = streakCoinPitchBase;
                bool level = false;

                // have run into a coin, collect it!
                pawnPurse.AddCoins(coin.coinValue);

                streakCoins++;
                if (streakCoins >= streakCoinsLevel + streakCoinsLevelNext)
                {
                    streakLevel++;
                    streakCoinsLevel = streakCoinsLevel + streakCoinsLevelNext;
                    streakCoinsLevelNext += streakCountPerLevel;

                    level = true;
                }
                if (streakLevel > 0)
                {
                    float streakFill = (float)(streakCoins - streakCoinsLevel) / (float)streakCoinsLevelNext;
                    streakDisplay.StreakUpdate(streakLevel, streakFill);
                    pitch += streakLevel * streakCoinPitchPerLevel + streakFill * streakCoinPitchForProgress;
                    if (level)
                        AudioManager.instance.SoundPlayCustom(streakLevelSound, Vector2.zero, 1f, pitch);
                }

                coin.CollectedCoin(pitch);
            }
        }
        else
        {
            CollectibleWeapon weapon = other.gameObject.GetComponent<CollectibleWeapon>();

            if (weapon)
            {
                if (weapon.unused)
                {
                    pawnWeapon.GetWeapon(weapon);
                    weapon.Collected();
                }
            }
            else
            {
                CollectiblePotion potion = other.gameObject.GetComponent<CollectiblePotion>();

                if (potion)
                {
                    if (potion.unused)
                    {
                        pawnHealth.GainHealth(1f);
                        potion.Collected();
                    }
                }
                else
                {
                    HazardBase hazard = other.gameObject.GetComponent<HazardBase>();

                    if (hazard && hazard.awake)
                    {
                        if (!pawnLoco.jumping)
                        {
                            // have run into a hazard!
                            float takeDamage = 1f;

                            if (pawnWeapon.weapon)
                            {
                                float hazardStrength = pawnWeapon.UseWeapon(takeDamage);

                                // possibly move this to the weapon itself
                                switch (Random.Range(0, 3))
                                {
                                    default:
                                    case 0:
                                        {
                                            pawnLoco.pawnAnim.SetTrigger("Attack1");
                                            break;
                                        }
                                    case 1:
                                        {
                                            pawnLoco.pawnAnim.SetTrigger("Attack2");
                                            break;
                                        }
                                    case 2:
                                        {
                                            pawnLoco.pawnAnim.SetTrigger("Attack3");
                                            break;
                                        }
                                }

                                if (hazardStrength > 0)
                                {
                                    // the hazard was not defeated
                                    takeDamage = hazardStrength;
                                }
                                else
                                {
                                    // the hazard was defeated
                                    takeDamage = 0f;
                                }
                            }
                            else
                            {
                                pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
                            }

                            speed = 0f;

                            if (takeDamage > 0)
                            {
                                hazard.Collided();
                                pawnHealth.TakeDamage(takeDamage);
                                StreakEnd();
                            }
                            else
                            {
                                hazard.Defeated();
                            }
                        }
                    }
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

        pointer.ShowPointer(touchPos);

        // convert the touch inside the main area to a movement position
        pawnTargetX = minX + (maxX - minX) * touchX;
        pawnLoco.SetMoveTarget(pawnTargetX);

        if (touchY > screenJumpY)
        {
            // jump input received
            if (!jumpHeld)
                jumpHeld = pawnLoco.StartJump();
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
            if (Input.GetAxis("Horizontal") != 0)
            {
                pawnTargetX = Mathf.Clamp(transform.position.x + (Input.GetAxis("Horizontal") * pawnLoco.moveRate * Time.deltaTime), minX, maxX);
                pawnLoco.SetMoveTarget(pawnTargetX);
            }
            if (Input.GetButton("Jump"))
            {
                if (!jumpHeld)
                {
                    jumpHeld = pawnLoco.StartJump();
                }
            }
            else
            {
                jumpHeld = false;
            }
        }
    }
}
