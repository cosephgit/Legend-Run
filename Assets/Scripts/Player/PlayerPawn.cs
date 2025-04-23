using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// takes player input and moves the player pawn depending on input
// created 18/8/23
// last modified 5/9/23


/*
 * so working out the tutorial...
 * at the start of the level, tell the player how to move (this is an autopass as soon as the player taps)
 * then spawn some coins and tell the player to move to grab them
 * then "well done" after they grab some of them (not all)
 * then spawn a hazard and tell the player to dodge them
 * then "well done" when they dodge successfully
 * then spawn a hazard and tell the player to jump it (lock their position into the hazard line)
 * then "well done" when they jump when the hazard passes
 */


public class PlayerPawn : MonoBehaviour
{
    public static PlayerPawn instance;
    [Header("Components")]
    [SerializeField] private PlayerPawnHealth pawnHealth;
    [SerializeField] private PlayerPawnLoco pawnLoco;
    [field: SerializeField] public PlayerPawnPurse pawnPurse { get; private set; }
    [SerializeField] private PlayerWeapon pawnWeapon;
    [field: SerializeField] public PlayerTutorial tutorial { get; private set; }
    [Header("Key references")]
    [SerializeField] private TerrainManager terrain;
    [SerializeField] private UIStreak streakDisplay;
    [Header("Movement parameters")]
    [SerializeField] private float speedMax = 16f;
    [SerializeField] private float speedAccel = 6f;
    //[SerializeField] private float swipeSensitivity = 0.05f;
    //[Header("Input parameters")]
    //[SerializeField] private float screenActiveXMin = 0.2f; // left side main zone of screen
    //[SerializeField] private float screenActiveXMax = 0.8f; // right side main zone of screen
    //[SerializeField] private float screenJumpY = 0.5f; // minimum screen height to jump
    [Header("Streak params")]
    [SerializeField] private int streakCountBase = 5;
    [SerializeField] private int streakCountPerLevel = 5;
    [SerializeField] private AudioClip streakLevelSound;
    [SerializeField] private AudioClip streakFailSound;
    [SerializeField] private float streakCoinPitchBase = 1f;
    [SerializeField] private float streakCoinPitchPerLevel = 0.05f;
    [SerializeField] private float streakCoinPitchForProgress = 0.2f;
    [SerializeField] private float streakSpeedBoostPerLevel = 0.1f;
    [Header("Effect prefabs")]
    [SerializeField] private TimedEffect tusslePrefab;
    [Header("Speed lines")]
    [SerializeField] private ParticleSystem speedLines;
    [SerializeField] private float speedLineSpeedPerSpeed = 0.5f;
    private float pawnTargetX; // the pawn's current X target
    private float speed;
    // for PC testing
    private bool jumpHeld;
    private bool rightHeld;
    private bool leftHeld;
    // streak values
    private float streakCoins;
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
    }

    private void Start()
    {
        pawnTargetX = transform.position.x;
        speed = 0;
        streakLevel = GameManager.instance.upgrades.upgradeBoostInitial;
        streakCoins = StreakCoinsForLevel(streakLevel);
        StreakSetSpeed();
        speedLines.Stop();
        speedLinesEmission = speedLines.emission;
        speedLinesEmissionRateBase = speedLinesEmission.rateOverTime.constant;
        speedLinesMain = speedLines.main;
        speedLinesMainSpeedBase = speedLinesMain.startSpeed.constant;

        tutorial.Initialise();
    }

    public void TutorialStart()
    {
        streakLevel = GameManager.instance.upgrades.upgradeBoostInitial;
        streakCoins = StreakCoinsForLevel(streakLevel);
        pawnHealth.PreTutorial();
    }


    // called at the start of the stage, or at the end of the tutorial
    public void StageStart()
    {
        streakLevel = GameManager.instance.upgrades.upgradeBoostInitial;
        streakCoins = StreakCoinsForLevel(streakLevel);
        pawnHealth.Initialise();
    }

    public void StreakEnd()
    {
        if (streakLevel > GameManager.instance.upgrades.upgradeBoostInitial
            || (streakLevel > 0 && streakCoins > StreakCoinsForLevel(streakLevel)))
        {
            streakDisplay.StreakBreak();
            AudioManager.instance.SoundPlayEven(streakFailSound, Vector2.zero);
        }
        streakLevel = GameManager.instance.upgrades.upgradeBoostInitial;
        streakCoins = StreakCoinsForLevel(streakLevel);
        StreakSetSpeed();
        if (speed > speedMax)
            SetSpeed(speedMax);
        speedLines.Stop();
    }

    private void StreakSetSpeed()
    {
        streakSpeedBonus = 1f + (streakLevel * streakSpeedBoostPerLevel);
        pawnLoco.SetSpeedBoost(streakSpeedBonus);
    }
    private int StreakCoinsForLevel(int level)
    {
        return (streakCountBase * level + streakCountPerLevel * (level - 1) * level / 2);
    }
    private void TouchCoin(CollectibleCoin coin)
    {
        if (coin.unused)
        {
            int streakCoinsLevel = StreakCoinsForLevel(streakLevel); // number of coins to the start of this level
            int streakCoinsLevelNext = streakCountBase + streakCountPerLevel * streakLevel; // number of coins from the start of this level to the next level
            float pitch = streakCoinPitchBase;
            bool level = false;

            if (tutorial.state == TutorialState.Finished)
            {
                // have run into a coin, collect it!
                GameManager.instance.AddCoins(coin.coinValue);
                pawnPurse.ChangeBars();

                streakCoins += coin.coinValue * GameManager.instance.upgrades.upgradeBoostGainRate;
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
                        StreakSetSpeed();
                    }
                }
            }
            else // don't do streaks or anything else, just progress the tutorial if appropriate
                tutorial.CoinCollected();

            coin.CollectedCoin(pitch);
        }
    }
    private void TouchGem(CollectibleGem gem)
    {
        if (gem.unused)
        {
            GameManager.instance.AddGems(1);
            pawnPurse.ChangeBars();

            gem.CollectedGem();
        }
    }
    private void TouchHazard(HazardBase hazard)
    {
        if (hazard.awake)
        {
            if (!pawnLoco.jumping)
            {
                // have run into a hazard!
                int takeDamage = 1;

                if (pawnWeapon.weapon && hazard.killable)
                {
                    int hazardStrength = pawnWeapon.UseWeapon(takeDamage);

                    if (hazardStrength > 0)
                    {
                        // the hazard was not defeated
                        takeDamage = hazardStrength;
                    }
                    else
                    {
                        // the hazard was defeated
                        takeDamage = 0;
                    }
                }

                SetSpeed(0f);
                speedLines.Stop();
                Instantiate(tusslePrefab, hazard.transform.position, hazard.transform.rotation, hazard.transform);

                if (takeDamage > 0)
                {
                    hazard.Collided();
                    if (tutorial.state == TutorialState.Finished)
                    {
                        pawnHealth.TakeDamage(takeDamage);
                        StreakEnd();
                    }
                    else
                        tutorial.HazardHit();

                    if (pawnHealth.IsAlive())
                    {
                        pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
                    }
                    else
                    {
                        // defeat!
                        pawnLoco.pawnAnim.SetBool("Dead", true);
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
    private void TouchWeapon(CollectibleWeapon weapon)
    {
        if (weapon.unused)
        {
            pawnWeapon.GetWeapon(weapon);
            weapon.Collected();
        }
    }
    private void TouchPotion(CollectiblePotion potion)
    {
        if (potion.unused)
        {
            pawnHealth.GainHealth(1);
            potion.Collected();
        }
    }

    // handles touches with obstacles and coins and powerups
    // TODO maybe rewrite this, it's a bit chonky
    private void OnTriggerEnter(Collider other)
    {
        if (!pawnHealth.IsAlive()) return;

        CollectibleCoin coin = other.gameObject.GetComponent<CollectibleCoin>();
        if (coin)
        {
            TouchCoin(coin);
            return;
        }

        CollectibleWeapon weapon = other.gameObject.GetComponent<CollectibleWeapon>();
        if (weapon)
        {
            TouchWeapon(weapon);
            return;
        }

        CollectiblePotion potion = other.gameObject.GetComponent<CollectiblePotion>();
        if (potion)
        { 
            TouchPotion(potion);
            return;
        }

        HazardBase hazard = other.gameObject.GetComponent<HazardBase>();
        if (hazard)
        {
            TouchHazard(hazard);
            return;
        }

        CollectibleGem gem = other.gameObject.GetComponent<CollectibleGem>();
        if (gem)
        {
            TouchGem(gem);
        }
    }

    // takes the screen position as input (from a touch or mouse) and converts it to a movement instruction
    public void SetMove(Vector2 touchPos)
    {
        return;
        /*
        // don't assign moves while in pause menu!
        if (TerrainManager.instance.paused || TerrainManager.instance.defeat || TerrainManager.instance.victory) return;

        if (!pawnHealth.IsAlive()) return;

        // get the touch position in the main action area
        float touchX = (Mathf.Clamp(touchPos.x / Screen.width, screenActiveXMin, screenActiveXMax) - screenActiveXMin) / (screenActiveXMax - screenActiveXMin);
        float touchY = touchPos.y / Screen.height;

        if (touchY > screenJumpY)
        {
            if (tutorial.state == TutorialState.Jump || tutorial.state == TutorialState.Finished)
            {
                // jump input received
                if (!jumpHeld)
                    jumpHeld = pawnLoco.StartJump();
            }
            else return;
        }
        else
        {
            if (tutorial.state == TutorialState.Init || tutorial.state == TutorialState.Jump) return;

            tutorial.PlayerMoved();

            jumpHeld = false;
        }

        // convert the touch inside the main area to a movement position
        pawnTargetX = TerrainManager.instance.moveMinX + (TerrainManager.instance.moveMaxX - TerrainManager.instance.moveMinX) * touchX;
        pawnLoco.SetMoveTarget(pawnTargetX);
        */
    }

    // receive a swipe instruction
    public void SetSwipe(Vector3 swipe)
    {
        //Debug.Log("Received a swipe! x " + swipe.x + " y " + swipe.y);

        if (pawnHealth.IsAlive() && !TerrainManager.instance.paused)
        {
            if (swipe.y > 0f)
            {
                //Debug.Log("Swiped up!");
                if (tutorial.state == TutorialState.Finished
                    || tutorial.state == TutorialState.Jump
                    || tutorial.state == TutorialState.JumpDone
                    || tutorial.state == TutorialState.JumpOver
                    || tutorial.state == TutorialState.JumpOverDone)
                {
                    jumpHeld = pawnLoco.StartJump();
                    if (jumpHeld)
                        tutorial.PlayerJumped();
                }
            }
            else if (swipe.x > 0)
            {
                //Debug.Log("Swiped right!");
                if (tutorial.state == TutorialState.Init || tutorial.state == TutorialState.Jump) return;

                if (pawnTargetX >= TerrainManager.instance.laneRightXMin)
                    return;

                if (pawnTargetX > TerrainManager.instance.laneLeftXMax)
                    pawnTargetX = TerrainManager.instance.laneRightX;
                else
                    pawnTargetX = TerrainManager.instance.laneCentreX;

                pawnLoco.SetMoveTarget(pawnTargetX);
                tutorial.PlayerMoved();
            }
            else if (swipe.x < 0)
            {
                //Debug.Log("Swiped left!");
                if (tutorial.state == TutorialState.Init || tutorial.state == TutorialState.Jump) return;

                if (pawnTargetX <= TerrainManager.instance.laneLeftXMax)
                    return;

                if (pawnTargetX < TerrainManager.instance.laneRightXMin)
                    pawnTargetX = TerrainManager.instance.laneLeftX;
                else
                    pawnTargetX = TerrainManager.instance.laneCentreX;

                pawnLoco.SetMoveTarget(pawnTargetX);
                tutorial.PlayerMoved();
            }
            /*
            if (swipe.sqrMagnitude > swipeSensitivity * swipeSensitivity)
            {
                if (swipe.y > Mathf.Abs(swipe.x))
                {
                    //Debug.Log("Swiped up!");
                    if (tutorial.state == TutorialState.Finished
                        || tutorial.state == TutorialState.Jump
                        || tutorial.state == TutorialState.JumpDone
                        || tutorial.state == TutorialState.JumpOver
                        || tutorial.state == TutorialState.JumpOverDone)
                    {
                        jumpHeld = pawnLoco.StartJump();
                        if (jumpHeld)
                            tutorial.PlayerJumped();
                    }
                }
                else if (swipe.x > 0)
                {
                    //Debug.Log("Swiped right!");
                    if (tutorial.state == TutorialState.Init || tutorial.state == TutorialState.Jump) return;

                    if (pawnTargetX >= TerrainManager.instance.laneRightXMin)
                        return;

                    if (pawnTargetX > TerrainManager.instance.laneLeftXMax)
                        pawnTargetX = TerrainManager.instance.laneRightX;
                    else
                        pawnTargetX = TerrainManager.instance.laneCentreX;

                    pawnLoco.SetMoveTarget(pawnTargetX);
                    tutorial.PlayerMoved();
                }
                else
                {
                    //Debug.Log("Swiped left!");
                    if (tutorial.state == TutorialState.Init || tutorial.state == TutorialState.Jump) return;

                    if (pawnTargetX <= TerrainManager.instance.laneLeftXMax)
                        return;

                    if (pawnTargetX < TerrainManager.instance.laneRightXMin)
                        pawnTargetX = TerrainManager.instance.laneLeftX;
                    else
                        pawnTargetX = TerrainManager.instance.laneCentreX;

                    pawnLoco.SetMoveTarget(pawnTargetX);
                    tutorial.PlayerMoved();
                }
            }
            */
            //else
            //Debug.Log("Not really, just a tap!");
        }

        tutorial.TouchInput();
    }
    // called when a swipe is finished
    public void SwipeEnd(Vector3 swipe)
    {
        // doesn't do anything yet!
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
#if UNITY_EDITOR
        if (Input.GetButtonUp("Jump"))
            tutorial.TouchInput();
#elif UNITY_STANDALONE_WIN
        if (Input.GetButtonUp("Jump"))
            tutorial.TouchInput();
#endif

        if (!pawnHealth.IsAlive()) return;

        if (speed < speedMax * streakSpeedBonus)
        {
            // constantly increase the player's running speed
            SetSpeed(Mathf.Min(speed + speedAccel * Time.deltaTime * streakSpeedBonus, speedMax * streakSpeedBonus));
        }

        // temp for testing
        if (Input.touchCount == 0 && (!Input.mousePresent || !Input.GetMouseButton(0)))
        {
            if (rightHeld)
            {
                if (Input.GetAxis("Horizontal") <= 0)
                {
                    SetSwipe(new Vector3(1, 0f));
                    rightHeld = false;
                }
            }
            else
            {
                if (Input.GetAxis("Horizontal") > 0)
                    rightHeld = true;
            }

            if (leftHeld)
            {
                if (Input.GetAxis("Horizontal") >= 0)
                {
                    SetSwipe(new Vector3(-1, 0f));
                    leftHeld = false;
                }
            }
            else
            {
                if (Input.GetAxis("Horizontal") < 0)
                    leftHeld = true;
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

    // the player has passed a hazard - do tutorial handling
    public void HazardPassed()
    {
        if (tutorial.state == TutorialState.Finished) return;

        if (tutorial.state == TutorialState.JumpOver)
            tutorial.HazardJumped();
        else
            tutorial.HazardDodged();
    }
    public void LootMissed()
    {
        if (tutorial.state == TutorialState.Finished) return;

        if (tutorial.state == TutorialState.Collect)
            tutorial.CoinMissed();
    }

    // used during initialisation to work out how much X movement the player can achieve for each Z movement (at full speed)
    public float PlayerXPerZ()
    {
        return pawnLoco.moveRate / speedMax;
    }

    // get back up after defeat
    public void PlayerRecover()
    {
        pawnLoco.pawnAnim.SetBool("Dead", false);
        pawnLoco.pawnAnim.SetTrigger("DamageHeavy");
        pawnHealth.FillHealth();
    }
}
