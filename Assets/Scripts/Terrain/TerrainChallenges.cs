using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TerrainChallenges
// this class manages the challenges in the game - collectibles, bonuses, and hazards
// this is procedurally generated with a number of patterns
// created 19/8/23
// last modified 16/7/24


public class TerrainChallenges : MonoBehaviour
{
    [Header("Spawning bounds")]
    [SerializeField] private float challengeZRemoval = -1f;
    [Header("Coins")]
    [SerializeField] private CollectibleCoin[] coins; // coins to spawn - TODO make a custom class with value by coin type
    [SerializeField] private CollectibleGem gemPrefab; // coins to spawn - TODO make a custom class with value by coin type
    [SerializeField] private float coinsPerDistanceBase = 0.2f; // the number of coins spawned for each unit of distance per point of difficulty
    [SerializeField] private float coinsPerDistancePerDifficulty = 0.1f; // the number of coins spawned for each unit of distance per point of difficulty
    [SerializeField] private float coinsAverageWorthBase = 1.1f;
    [SerializeField] private float coinsAverageWorthPerDifficulty = 0.2f;
    [SerializeField] private int coinsMin = 5; // minimum number of coins appearing in one stretch
    [SerializeField] private int coinsMax = 10; // minimum number of coins appearing in one stretch
    [SerializeField] private float coinHeight = 1f;
    [SerializeField] private float coinGapZ = 1f; // the space between coins in a chain
    [SerializeField] private float spawnGroupGapZ = 5f; // the minimum gap between two runs of coins or powerups
    [SerializeField] private float gemGap = 1000f;
    [Header("hazards")]
    [SerializeField] private HazardBase[] hazards; // hazards to spawn - TODO make a custom class with danger by hazard type
    [SerializeField] private float hazardPerDistanceBase = 0f; // the number of hazards spawned for each unit of distance per point of difficulty
    [SerializeField] private float hazardPerDistancePerDifficulty = 0.05f; // the number of hazards spawned for each unit of distance per point of difficulty
    [SerializeField] private int hazardMin = 1;
    [SerializeField] private int hazardMax = 3;
    [SerializeField] private float hazardXGap = 2f; // the minimum gap between hazards
    [SerializeField] private float hazardXBuffer = 0.5f; // maximum amount hazards can go outside the normal play bounds
    [SerializeField] private float hazardBlockSpawnShadow = 5f; // minimum distance after spawning a full blockage before allowing any powerups
    [SerializeField] private float hazardPowerupAvoidSpace = 0.1f; // if the safe route has moved this much out of lane, treat it as already being in the next lane for hazard spawning
    [SerializeField] private float hazardStrengthPerDifficulty = 0.1f;
    [Header("powerups")]
    [SerializeField] private CollectibleBase[] powerups;
    [SerializeField] private float powerupPerDistanceBase = 0.01f;
    [SerializeField] private float powerupPerDistancePerDifficulty = 0.01f;
    [SerializeField] private float powerupGapX = 4f; // the minimum gap between powerups
    [Header("Intensity")]
    [SerializeField] private float intensityChangeZMin = 10f;
    [SerializeField] private float intensityChangeZMax = 30f;
    [SerializeField] private float intensityMax = 2f;
    private float difficulty = 0;
    private float coinsPerDistanceCurrent;
    private float coinsWorthCurrent;
    private float hazardPerDistanceCurrent;
    private float powerupPerDistanceCurrent;
    private float coinsAcc; // accumulated coin points
    private float hazardAcc; // accumulated hazard points
    private float powerupAcc; // accumulated hazard points
    private float baseHeight;
    private Quaternion baseRot;
    // coin chain variables
    private int coinsLeft; // coins left to spawn in the current chain of coins
    private int coinsWorthLeft; // coins left to spawn in the current chain of coins
    private float coinPowerupNextZ; // the Z distance left before the next coin or powerup is spawned
    private float coinLastPlaced; // distance since the last coin was placed (certain obstacles require a gap)
    private float gemNextZ;
    // hazard spawn variables
    private float hazardNextZ; // the Z distance left before the next hazard can be spawned
    // intensity tracking
    private float intensityCurrent; // the current intensity level
    private float intensityTarget; // the target intensity level before the next intensity shift
    private float intensityChangeZ; // z movement before the next shift in intensity
    // object records
    private List<CollectibleCoin> coinsActive;
    private List<CollectibleGem> gemsActive;
    private List<HazardBase> hazardsActive;
    private List<CollectibleBase> powerupsActive;

    private void Awake()
    {
        // for testing
        SetDifficulty(0f);
        coinsActive = new List<CollectibleCoin>();
        gemsActive = new List<CollectibleGem>();
        hazardsActive = new List<HazardBase>();
        powerupsActive = new List<CollectibleBase>();
        coinsAcc = 0f;
        gemNextZ = gemGap;
        hazardAcc = -1f; // ensure coins always appear first before any hazards
        powerupAcc = -1f; // or powerups
    }
    // initialise the terrainchallenges with the parameters from the main terrain manager
    // circleheight is the size of the rotating circle track (for positioning) and the rotation of the origin point (the furthest terrain that is out of sight)
    public void Initialise(float circleheight, Quaternion rot)
    {
        baseHeight = circleheight;
        baseRot = rot;
        IntensityStart();
    }
    // updates with the current difficulty level
    public void SetDifficulty(float diff)
    {
        difficulty = diff;
        coinsPerDistanceCurrent = coinsPerDistanceBase + (difficulty * coinsPerDistancePerDifficulty);
        hazardPerDistanceCurrent = hazardPerDistanceBase + (difficulty * hazardPerDistancePerDifficulty);
        powerupPerDistanceCurrent = powerupPerDistanceBase + (difficulty * powerupPerDistancePerDifficulty);
        coinsWorthCurrent = coinsAverageWorthBase + (difficulty * coinsAverageWorthPerDifficulty);
        IntensityStart();
    }
    // starts the intensity cycle
    private void IntensityStart()
    {
        intensityCurrent = 0f;
        intensityTarget = intensityMax;
        intensityChangeZ = Random.Range(intensityChangeZMin, intensityChangeZMax);
    }

    // called by the TerrainManager in each Update to add the distance travelled for the frame
    public void AddDistance(float dist, float safeCurrentX, bool special)
    {
        if (PlayerPawn.instance.tutorial.state == TutorialState.Finished)
        {
            // tutorial has finished, normal level behaviour
            coinsAcc += dist * coinsPerDistanceCurrent * intensityCurrent;
            hazardAcc += dist * hazardPerDistanceCurrent * intensityCurrent;
            powerupAcc += dist * powerupPerDistanceCurrent * (intensityMax - intensityCurrent); // powerups work inversely with intensity - appearing most in lulls rather than peaks
        }
        else
        {
            // tutorial-only behaviour
            if (PlayerPawn.instance.tutorial.state == TutorialState.Collect)
                coinsAcc += dist * coinsPerDistanceCurrent * 0.5f;
            else if (PlayerPawn.instance.tutorial.state == TutorialState.Dodge
                || PlayerPawn.instance.tutorial.state == TutorialState.Jump)
                hazardAcc += dist * hazardPerDistanceCurrent * 0.5f;
        }

        coinPowerupNextZ -= dist;
        coinLastPlaced += dist;
        hazardNextZ -= dist;
        gemNextZ -= dist;
        if (intensityChangeZ > 0)
        {
            // gradually ramp the intensity up to the maximum value, then drop for another gradual rise
            if (intensityChangeZ < dist)
                intensityCurrent = intensityTarget;
            else
                intensityCurrent += (intensityTarget - intensityCurrent) * dist / intensityChangeZ;
            intensityChangeZ -= dist;
        }
        else
        {
            IntensityStart();
        }

        PlaceChallenges(safeCurrentX, special);
    }

    // have coins accumulated, set up a coin chain to start spawning them
    private void StartCoinChain()
    {
        if (PlayerPawn.instance.tutorial.state == TutorialState.Collect)
        {
            coinsLeft = coinsMin;
            coinsWorthLeft = coinsLeft;
        }
        else
        {
            coinsLeft = Mathf.FloorToInt(Mathf.Lerp(coinsMin, coinsMax, intensityCurrent / 2f));
            coinsWorthLeft = Mathf.CeilToInt(coinsLeft * Mathf.Max(1f, coinsWorthCurrent * intensityCurrent));
        }

        coinsAcc -= coinsWorthLeft;
        coinPowerupNextZ = spawnGroupGapZ;
    }

    // place a single coin in the current coin chain
    private void PlaceCoin(float safeCurrentX)
    {
        float laneX;

        if (safeCurrentX <= TerrainManager.instance.laneLeftXMax)
            laneX = TerrainManager.instance.laneLeftX;
        else if (safeCurrentX >= TerrainManager.instance.laneRightXMin)
            laneX = TerrainManager.instance.laneRightX;
        else
            laneX = TerrainManager.instance.laneCentreX;

        Vector3 pos = transform.position + baseRot * new Vector3(laneX, baseHeight + coinHeight, 0f);
        int coinIndex = 0;
        int coinsWorthExcess = coinsWorthLeft - coinsLeft;

        // coin selection to give variety in coin chains
        if (coinsWorthLeft / coinsLeft > 8f) coinIndex = 2; // definitely want to start dropping gold
        else if (coinsWorthLeft / coinsLeft > 2f) coinIndex = 1; // definitely want to start dropping silver
        else
        {
            if (coinsWorthExcess > 4)
            {
                // have enough excess to consider a gold
                if (Random.Range(1f, 4f) < coinsWorthExcess / coinsLeft)
                    coinIndex = 2;
            }
            if (coinIndex == 0 && coinsWorthExcess > 0)
            {
                // have enough excess to consider a silver
                if (Random.Range(0f, 1f) < coinsWorthExcess / coinsLeft)
                    coinIndex = 1;

            }
        }

        CollectibleCoin coin = Instantiate(coins[coinIndex], pos, baseRot, transform);

        coinsWorthLeft -= coin.coinValue;
        if (coinsWorthLeft > 0)
            coinsLeft--;
        else
        {
            coinsAcc += coinsWorthLeft; // if this is negative it means we spawned more value of coins than intended, so adjust the accumulator
            coinsLeft = 0;
        }

        coinLastPlaced = 0;
        coinsActive.Add(coin);
    }

    // place a single coin in the current coin chain
    private void PlaceGem(float safeCurrentX)
    {
        float laneX;

        if (safeCurrentX <= TerrainManager.instance.laneLeftXMax)
            laneX = TerrainManager.instance.laneLeftX;
        else if (safeCurrentX >= TerrainManager.instance.laneRightXMin)
            laneX = TerrainManager.instance.laneRightX;
        else
            laneX = TerrainManager.instance.laneCentreX;

        Vector3 pos = transform.position + baseRot * new Vector3(laneX, baseHeight + coinHeight, 0f);

        CollectibleGem gem = Instantiate(gemPrefab, pos, baseRot, transform);

        gemsActive.Add(gem);
    }

    // calculates positions for hazards, mainly for events when 2 or 3 hazards are being spawned in one row
    private float CalcHazardX(float leftmost, float rightmost, float maxJump)
    {
        // hazardSpawnSafeX to do
        float returnX;
        float maxJumpLeft = 0f;
        float maxJumpRight = 0f;

        if (maxJump >= 1f)
        {
            // this placement can jump so check which ways it can jump
            if (leftmost - hazardXGap * 2f >= TerrainManager.instance.moveMinX - hazardXBuffer)
                maxJumpLeft = Mathf.Min(hazardXBuffer - TerrainManager.instance.moveMinX + leftmost, (1f + maxJump) * hazardXBuffer);
            if (rightmost + hazardXGap * 2f <= TerrainManager.instance.moveMaxX + hazardXBuffer)
                maxJumpRight = Mathf.Min(hazardXBuffer + TerrainManager.instance.moveMaxX - rightmost, (1f + maxJump) * hazardXBuffer);
        }

        if ((maxJumpLeft > 0f || maxJumpRight > 0f) && CoSephUtils.RandomBool())
        {
            // place the next hazard spaced away from the first
            if (Random.Range(-maxJumpLeft, maxJumpRight) > 0) // right jump
                returnX = rightmost + Random.Range(hazardXGap * 2f, maxJumpRight);
            else // left jump
                returnX = leftmost - Random.Range(hazardXGap * 2f, maxJumpLeft);
        }
        else
        {
            // place the second hazard directly adjacent to the first
            bool right;
            if (rightmost + hazardXGap > TerrainManager.instance.moveMaxX + hazardXBuffer)
                right = false; // can't go right
            else if (leftmost - hazardXGap < TerrainManager.instance.moveMinX - hazardXBuffer)
                right = true; // can't go left
            else // could be either
                right = CoSephUtils.RandomBool();

            if (right)
                returnX = rightmost + hazardXGap;
            else
                returnX = leftmost - hazardXGap;
        }

        return returnX;
    }

    // place the required number of hazards
    // need to make sure that these are spaced well to allow a gap for the player
    private void PlaceHazard(float safeCurrentX)
    {
        float spawnStrength = (Random.Range(0.0f, 1.0f) + (difficulty * hazardStrengthPerDifficulty)) * intensityCurrent / intensityMax;
        int hazardSpawn = Mathf.Clamp(Mathf.FloorToInt(Mathf.Lerp(hazardMin, hazardMax, spawnStrength)), 1, 3);
        float[] hazardX;
        HazardBase hazardType = hazards[Random.Range(0, hazards.Length)];

        if (PlayerPawn.instance.tutorial.state == TutorialState.Dodge)
        {
            hazardSpawn = 2; // always place maximum to make the player actively evade
            hazardType = hazards[0];
        }
        else if (PlayerPawn.instance.tutorial.state == TutorialState.Jump)
        {
            hazardSpawn = 3; // block the path - only way past is to jump
            hazardType = hazards[0];
        }
        else if (hazardSpawn == 3 && coinLastPlaced < spawnGroupGapZ) // enforce a minimum coin gap before a three-lane blockage
            hazardSpawn = 2;

        // the first hazard is always adjacent to the safe path
        hazardX = new float[hazardSpawn];

        /*
            * PSEUDO
            * if safeCurrent is CENTRE: spawn on one side or the other
            * if safeCurrent is RIGHT: spawn centre
            * if safeCurrent is LEFT: spawn centre
            */

        if (hazardSpawn == 3)
        {
            hazardX[0] = TerrainManager.instance.laneLeftX;
            hazardX[1] = TerrainManager.instance.laneCentreX;
            hazardX[2] = TerrainManager.instance.laneRightX;
            // make sure coins/powerups don't spawn right behind a full block to give player time to land after jumping
            if (coinPowerupNextZ < hazardBlockSpawnShadow)
                coinPowerupNextZ = hazardBlockSpawnShadow;
        }
        else
        {
            /*
             * situations:
             * in left lane
             * * fixed/shifting left
             * - LEFT LANE CLEAR, OTHER LANES POSSIBLE
             * * shifting right
             * - LEFT AND CENTRE LANE CLEAR, RIGHT LANE BLOCK ONLY
             * 
             * in centre lane
             * * fixed
             * - CENTRE CLEAR, LEFT AND RIGHT BLOCKABLE
             * * shifting left
             * - LEFT AND CENTRE CLEAR, RIGHT BLOCK ONLY
             * * shifting right
             * - RIGHT AND CENTRE CLEAR, LEFT BLOCK ONLY
             * 
             * in right lane
             ** shifting left
             * - RIGHT AND CENTRE LANE CLEAR, LEFT LANE BLOCK ONLY
             ** fixed/shifting right
             * - RIGHT LANE CLEAR, OTHER LANES POSSIBLE
             */

            bool rightAllowed;
            bool leftAllowed;
            bool centreAllowed;
            int max;


            if (safeCurrentX > TerrainManager.instance.laneCentreX + hazardPowerupAvoidSpace)
            {
                if (safeCurrentX < TerrainManager.instance.laneRightX - hazardPowerupAvoidSpace)
                {
                    // to/from right lane
                    rightAllowed = false;
                    centreAllowed = false;
                    leftAllowed = true;
                    max = 1;
                }
                else
                {
                    // in right lane
                    rightAllowed = false;
                    centreAllowed = true;
                    leftAllowed = true;
                    max = 2;
                }
            }
            else if (safeCurrentX < TerrainManager.instance.laneCentreX - hazardPowerupAvoidSpace)
            {
                if (safeCurrentX > TerrainManager.instance.laneLeftX + hazardPowerupAvoidSpace)
                {
                    // to/from left lane
                    rightAllowed = true;
                    centreAllowed = false;
                    leftAllowed = false;
                    max = 1;
                }
                else
                {
                    // in right lane
                    rightAllowed = true;
                    centreAllowed = true;
                    leftAllowed = false;
                    max = 2;
                }
            }
            else
            {
                centreAllowed = false;
                rightAllowed = true;
                leftAllowed = true;
                max = 2;
            }

            if (max < hazardSpawn)
            {
                hazardSpawn = max;
                hazardX = new float[max];
            }

            if (max == 2 && hazardSpawn == 1)
            {
                // always favour the current target X as the blockage lane, if not the current X
                if (TerrainManager.instance.safeTargetX <= TerrainManager.instance.laneLeftXMax)
                {
                    if (leftAllowed)
                    {
                        rightAllowed = false;
                        centreAllowed = false;
                    }
                }
                else if (TerrainManager.instance.safeTargetX >= TerrainManager.instance.laneRightXMin)
                {
                    if (rightAllowed)
                    {
                        leftAllowed = false;
                        centreAllowed = false;
                    }
                }
                else
                {
                    if (centreAllowed)
                    {
                        leftAllowed = false;
                        rightAllowed = false;
                    }
                }
                // else just pick randomly
                if (rightAllowed && leftAllowed)
                {
                    rightAllowed = CoSephUtils.RandomBool();
                    leftAllowed = !rightAllowed;
                }
                else if (rightAllowed && centreAllowed)
                {
                    rightAllowed = CoSephUtils.RandomBool();
                    centreAllowed = !rightAllowed;
                }
                else // leftAllowed && centreAllowed
                {
                    centreAllowed = CoSephUtils.RandomBool();
                    leftAllowed = !centreAllowed;
                }
            }

            if (hazardSpawn == 2)
            {
                if (rightAllowed)
                {
                    hazardX[0] = TerrainManager.instance.laneRightX;
                    if (leftAllowed)
                        hazardX[1] = TerrainManager.instance.laneLeftX;
                    else
                        hazardX[1] = TerrainManager.instance.laneCentreX;
                }
                else // must be left and centre
                {
                    hazardX[0] = TerrainManager.instance.laneLeftX;
                    hazardX[1] = TerrainManager.instance.laneCentreX;
                }
            }
            else // hazardSpawn == 1
            {
                if (rightAllowed)
                    hazardX[0] = TerrainManager.instance.laneRightX;
                else if (leftAllowed)
                    hazardX[0] = TerrainManager.instance.laneLeftX;
                else // centreAllowed
                    hazardX[0] = TerrainManager.instance.laneCentreX;
            }
        }

        /*
        hazardX[0] = CalcHazardX(safeCurrentX, safeCurrentX, 0);

        if (hazardSpawn == 2)
        {
            hazardX[1] = CalcHazardX(Mathf.Min(safeCurrentX, hazardX[0]), Mathf.Max(safeCurrentX, hazardX[0]), 10f);
        }
        if (hazardSpawn == 3)
        {
            hazardX[1] = CalcHazardX(Mathf.Min(safeCurrentX, hazardX[0]), Mathf.Max(safeCurrentX, hazardX[0]), 1f);
            hazardX[2] = CalcHazardX(Mathf.Min(safeCurrentX, hazardX[0], hazardX[1]), Mathf.Max(safeCurrentX, hazardX[0], hazardX[1]), 10f);
        }
        */

        hazardAcc -= hazardSpawn * hazardType.hazardThreat;

        for (int i = 0; i < hazardSpawn; i++)
        {
            Vector3 pos = transform.position + baseRot * new Vector3(hazardX[i], baseHeight, 0f);
            HazardBase hazard = Instantiate(hazardType, pos, baseRot, transform);
            hazardsActive.Add(hazard);
        }
    }

    private void PlacePowerup(float safeCurrentX)
    {
        float spawnStrength = intensityCurrent * Random.Range(0.5f, 1.5f);
        float laneX = TerrainManager.instance.GetLanePosX(safeCurrentX);
        Vector3 pos = transform.position + baseRot * new Vector3(laneX, baseHeight + coinHeight, 0f);
        CollectibleBase powerup = Instantiate(powerups[Random.Range(0, powerups.Length)], pos, baseRot, transform);
        powerupsActive.Add(powerup);

        powerupAcc -= 1f;
    }

    // place challenges for the latest distance update
    // safeCurrentX is the current X position of the projected safe walking path - coins appear here and hazards do not
    // special - this indicates this is a good position to start new coin chains/spawn powerups
    // final - the stage has nearly finished, just finish off the current coin chain
    private void PlaceChallenges(float safeCurrentX, bool special)
    {
        // if there are coins left: spawn coins
        if (coinsLeft > 0)
        {
            if (coinPowerupNextZ <= 0)
            {
                // currently in the middle of spawning a coin chain
                PlaceCoin(safeCurrentX);

                if (coinsLeft > 0)
                    coinPowerupNextZ = coinGapZ;
                else
                    coinPowerupNextZ = spawnGroupGapZ;
            }
        }
        else
        {
            // if not, consider placing a gem, powerup, or starting a coin chain
            if (coinPowerupNextZ < 0)
            {
                if (gemNextZ < 0)
                {
                    // place the next gem
                    PlaceGem(safeCurrentX);
                    gemNextZ += gemGap;
                }
                else if ((powerupAcc > 0 && special) || (powerupAcc > 2f))
                {
                    PlacePowerup(safeCurrentX);
                    coinPowerupNextZ = spawnGroupGapZ;
                }
                else if ((coinsAcc > 0 && special) || (coinsAcc > 2f))
                    StartCoinChain();
            }
        }

        if (hazardAcc > 0)
        {
            if (hazardNextZ <= 0 && !special)
            {
                PlaceHazard(safeCurrentX);
                hazardNextZ = spawnGroupGapZ;
            }
        }

        // remove items that have are behind the player
        if (coinsActive.Count > 0)
        {
            if (coinsActive[0] && coinsActive[0].unused)
            {
                if (coinsActive[0].transform.position.z < challengeZRemoval)
                {
                    coinsActive[0].Remove();
                    coinsActive.RemoveAt(0);
                    PlayerPawn.instance.StreakEnd();
                }
            }
            else
            {
                // TODO do this properly
                // remove a coin that has been deleted
                coinsActive.RemoveAt(0);
            }
        }
        if (gemsActive.Count > 0)
        {
            if (gemsActive[0] && gemsActive[0].unused)
            {
                if (gemsActive[0].transform.position.z < challengeZRemoval)
                {
                    gemsActive[0].Remove();
                    gemsActive.RemoveAt(0);
                }
            }
            else
                gemsActive.RemoveAt(0);
        }
        if (hazardsActive.Count > 0)
        {
            if (hazardsActive[0])
            {
                if (hazardsActive[0].transform.position.z < challengeZRemoval)
                {
                    if (hazardsActive[0].awake)
                        PlayerPawn.instance.HazardPassed();
                    hazardsActive[0].Remove();
                    hazardsActive.RemoveAt(0);
                }
            }
            else
            {
                hazardsActive.RemoveAt(0);
            }
        }
        if (powerupsActive.Count > 0)
        {
            if (powerupsActive[0])
            {
                if (powerupsActive[0].transform.position.z < challengeZRemoval)
                {
                    powerupsActive[0].Remove();
                    powerupsActive.RemoveAt(0);
                }
            }
            else
            {
                // TODO do this properly
                // remove a coin that has been deleted
                powerupsActive.RemoveAt(0);
            }
        }
    }

    // remove all challenges that have been spawned, for the tutorial
    public void ClearChallenges()
    {
        for (int i = powerupsActive.Count - 1; i >= 0; i--)
        {
            Destroy(powerupsActive[i].gameObject);
            powerupsActive.RemoveAt(i);
        }
        for (int i = hazardsActive.Count - 1; i >= 0; i--)
        {
            Destroy(hazardsActive[i].gameObject);
            hazardsActive.RemoveAt(i);
        }
        for (int i = coinsActive.Count - 1; i >= 0; i--)
        {
            Destroy(coinsActive[i].gameObject);
            coinsActive.RemoveAt(i);
        }
        coinsLeft = 0;
        coinsWorthLeft = 0;
        coinsAcc = 0f;
        hazardAcc = -1f;
        powerupAcc = -1f;
    }
}
