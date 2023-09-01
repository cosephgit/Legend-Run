using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// takes player input and moves the player pawn depending on input
// created 18/8/23
// last modified 1/9/23

public class PlayerPawn : MonoBehaviour
{
    public static PlayerPawn instance;
    [Header("Components")]
    [SerializeField] private PlayerPawnHealth pawnHealth;
    [SerializeField] private PlayerPawnLoco pawnLoco;
    [SerializeField] private PlayerPawnPurse pawnPurse;
    [SerializeField] private PlayerWeapon pawnWeapon;
    [SerializeField] private TerrainManager terrain;
    [SerializeField] private UIStreak streakDisplay;
    [Header("Movement parameters")]
    [SerializeField] private float speedMax = 16f;
    [SerializeField] private float speedAccel = 6f;
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
    [SerializeField] private float streakSpeedBoostPerLevel = 0.1f;
    [Header("Speed lines")]
    [SerializeField] private ParticleSystem speedLines;
    [SerializeField] private float speedLineSpeedPerSpeed = 0.5f;
    private float pawnTargetX; // the pawn's current X target
    private float speed;
    private bool jumpHeld;
    // streak values
    private int streakCoins;
    private int streakLevel;
    private float streakSpeedBonus;
    ParticleSystem.EmissionModule speedLinesEmission;
    float speedLinesEmissionRateBase;
    ParticleSystem.MainModule speedLinesMain;
    float speedLinesMainSpeedBase;

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
        streakSpeedBonus = 1f;
        speedLines.Stop();
        speedLinesEmission = speedLines.emission;
        speedLinesEmissionRateBase = speedLinesEmission.rateOverTime.constant;
        speedLinesMain = speedLines.main;
        speedLinesMainSpeedBase = speedLinesMain.startSpeed.constant;
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
        streakSpeedBonus = 1f;
        pawnLoco.SetSpeedBoost(streakSpeedBonus);
        if (speed > speedMax)
            SetSpeed(speedMax);
        speedLines.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pawnHealth.IsAlive()) return;

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

                streakCoins += coin.coinValue;
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
                    {
                        AudioManager.instance.SoundPlayCustom(streakLevelSound, Vector2.zero, 1f, pitch);
                        streakSpeedBonus = 1f + (streakLevel * streakSpeedBoostPerLevel);
                        pawnLoco.SetSpeedBoost(streakSpeedBonus);
                    }
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

                            SetSpeed(0f);
                            speedLines.Stop();

                            if (takeDamage > 0)
                            {
                                hazard.Collided();
                                pawnHealth.TakeDamage(takeDamage);
                                StreakEnd();
                                if (pawnHealth.IsAlive())
                                {
                                    pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
                                }
                                else
                                {
                                    // defeat!
                                    pawnLoco.pawnAnim.SetTrigger("Die");
                                    TerrainManager.instance.PlayerDefeat();
                                }
                            }
                            else
                            {
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

                                hazard.Defeated();
                            }
                        }
                    }
                }
            }
        }
    }

    // takes the screen position as input (from a touch or mouse) and converts it to a movement instruction
    public void SetMove(Vector2 touchPos)
    {
        // don't assign moves while in pause menu!
        if (TerrainManager.instance.paused || TerrainManager.instance.defeat || TerrainManager.instance.victory) return;

        if (!pawnHealth.IsAlive()) return;

        // get the touch position in the main action area
        float touchX = (Mathf.Clamp(touchPos.x / Screen.width, screenActiveXMin, screenActiveXMax) - screenActiveXMin) / (screenActiveXMax - screenActiveXMin);
        float touchY = touchPos.y / Screen.height;

        // convert the touch inside the main area to a movement position
        pawnTargetX = TerrainManager.instance.moveMinX + (TerrainManager.instance.moveMaxX - TerrainManager.instance.moveMinX) * touchX;
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

    // sets speed effects for a new speed value
    private void SetSpeed(float speedNew)
    {
        speed = speedNew;
        pawnLoco.SetSpeed(speed / speedMax);
        terrain.SetTerrainSpeed(speed);

        if (speed > speedMax)
        {
            float speedLineScale = 1f + ((speed - speedMax) * speedLineSpeedPerSpeed);
            speedLinesEmission.rateOverTime = speedLinesEmissionRateBase * speedLineScale;
            speedLinesMain.startSpeed = speedLinesMainSpeedBase * speedLineScale;
            speedLines.Play();
        }
    }

    private void Update()
    {
        if (!pawnHealth.IsAlive()) return;
        
        if (speed < speedMax * streakSpeedBonus)
        {
            // constantly increase the player's running speed
            SetSpeed(Mathf.Min(speed + speedAccel * Time.deltaTime * streakSpeedBonus, speedMax * streakSpeedBonus));
        }

        // temp for testing
        if (Input.touchCount == 0 && (!Input.mousePresent || !Input.GetMouseButton(0)))
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                pawnTargetX = Mathf.Clamp(transform.position.x + (Input.GetAxis("Horizontal") * pawnLoco.moveRate * Time.deltaTime),
                    TerrainManager.instance.moveMinX, TerrainManager.instance.moveMaxX);
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

    // used during initialisation to work out how much X movement the player can achieve for each Z movement (at full speed)
    public float PlayerXPerZ()
    {
        return pawnLoco.moveRate / speedMax;
    }
}
