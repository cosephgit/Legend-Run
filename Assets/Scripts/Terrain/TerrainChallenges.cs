using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

// TerrainChallenges
// this class manages the challenges in the game - collectibles, bonuses, and hazards
// this is procedurally generated with a number of patterns
// created 19/8/23
// last modified 21/8/23

/*
 * types of spawn:
 * just some coins
 * just an hazard
 * basically the general pattern is: hazard - coins - hazard - coins - etc
 * however the hazards and coins are both optional, it can start with either and continue repeating and possibly skipping steps with each
 * they form a continuous line (straight or curved)
 * multiple patterns can occur concurrently
 * coins should ALWAYS be collectible, the spacing should always allow collecting every single coin if the player is good enough
 * so if there are multiple tracks, they should be spaced so the player can skip between them
 * in other words - it's only one track, and might skip from one position to another
 * hazards CAN appear in multiple places, as long as there is always a path through
 * 
 * SO the pattern is:
 * possible coins (straight line or diagonal line)
 * possible hazards (possibly one in line with the previous coins/coming coins, possibly one or two more to fill the space)
 * possible coins (straight line or diagonal line, possibly starting from the same or a different position)
 * loop until done
 */

public class TerrainChallenges : MonoBehaviour
{
    [SerializeField] private float challengeXMin = -2.5f;
    [SerializeField] private float challengeXMax = 2.5f;
    [SerializeField] private float challengeZRemoval = -1f;
    [Header("Coins")]
    [SerializeField] private CollectibleCoin[] coins; // coins to spawn - TODO make a custom class with value by coin type
    [SerializeField] private float coinsPerDistanceBase = 0.2f; // the number of coins spawned for each unit of distance per point of difficulty
    [SerializeField] private float coinsPerDistancePerDifficulty = 0.1f; // the number of coins spawned for each unit of distance per point of difficulty
    [SerializeField] private int coinsMin = 5; // minimum number of coins appearing in one stretch
    [SerializeField] private int coinsMax = 10; // minimum number of coins appearing in one stretch
    [SerializeField] private float coinHeight = 1f;
    [SerializeField] private float coinGap = 1f; // the space between coins
    [SerializeField] private float coinChanceAngled = 0.5f; // the proportion of coin chains that will run at an angle
    [SerializeField] private float coinAngledXMin = 2f; // the minimum x offset for an angled run of coins
    [SerializeField] private float coinAngledXMax = 4f; // the minimum x offset for an angled run of coins
    [SerializeField] private float coinRunGap = 5f; // the minimum gap between two runs of coins
    [Header("hazards")]
    [SerializeField] private HazardBase[] hazards; // hazards to spawn - TODO make a custom class with danger by hazard type
    [SerializeField] private float hazardPerDistanceBase = 0f; // the number of hazards spawned for each unit of distance per point of difficulty
    [SerializeField] private float hazardPerDistancePerDifficulty = 0.05f; // the number of hazards spawned for each unit of distance per point of difficulty
    [SerializeField] private int hazardMin = 1;
    [SerializeField] private int hazardMax = 3;
    [SerializeField] private float hazardXGap = 2f; // the minimum gap between hazards
    [Header("Intensity")]
    [SerializeField] private float intensityChangeZMin = 10f;
    [SerializeField] private float intensityChangeZMax = 30f;
    private float difficulty;
    private float coinsPerDistanceCurrent;
    private float hazardPerDistanceCurrent;
    private float coinsAcc; // accumulated coin points
    private float hazardAcc; // accumulated hazard points
    private float baseHeight;
    private Quaternion baseRot;
    // coin chain variables
    private int coinsLeft; // coins left to spawn in the current chain of coins
    private float coinCurrentX; // the current X offset for the last coin in the sequence
    private float coinTargetX; // the target X offset for the final coin in the sequence
    private float coinNextZ; // the Z distance left before the next coin is spawned
    // hazard spawn variables
    private int hazardsLeft;
    // intensity tracking
    private float intensityCurrent; // the current intensity level
    private float intensityTarget; // the target intensity level before the next intensity shift
    private float intensityChangeZ; // z movement before the next shift in intensity
    // object records
    private List<CollectibleCoin> coinsActive;
    private List<HazardBase> hazardsActive;

    private void Awake()
    {
        SetDifficulty(0f);
        coinsActive = new List<CollectibleCoin>();
        hazardsActive = new List<HazardBase>();
        coinsAcc = 0f;
        hazardAcc = -1f; // ensure coins always appear first before any hazards
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
        IntensityStart();
    }
    // starts the intensity cycle
    private void IntensityStart()
    {
        intensityCurrent = 0f;
        intensityTarget = 2f;
        intensityChangeZ = Random.Range(intensityChangeZMin, intensityChangeZMax);
    }
    // called by the TerrainManager in each Update to add the distance travelled for the frame
    public void AddDistance(float dist)
    {
        coinsAcc += dist * coinsPerDistanceCurrent * intensityCurrent;
        hazardAcc += dist * hazardPerDistanceCurrent * intensityCurrent;
        coinNextZ -= dist;
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
        //Debug.Log("intensity: " + intensityCurrent + " - coinsAcc: " + coinsAcc + " - hazardAcc: " + hazardAcc);
    }

    // have hazards accumulated, set up spawning
    private void StartHazardChain(bool keepCurrentX = false)
    {
        float spawnStrength = intensityCurrent * Random.Range(0.5f, 1.5f);
        hazardsLeft = Mathf.FloorToInt(Mathf.Lerp(hazardMin, hazardMax, spawnStrength * hazardAcc/ (float)hazardMax));
        hazardAcc -= hazardsLeft;
        coinNextZ = coinRunGap;
        if (keepCurrentX) // place them immediately (normally means appear at the end of a coin chain)
            coinCurrentX = coinTargetX;
        else
            coinCurrentX = Random.Range(challengeXMin, challengeXMax);
    }

    // have coins accumulated, set up a coin chain to start spawning them
    private void StartCoinChain(bool keepCurrentX = false)
    {
        float spawnStrength = intensityCurrent * Random.Range(0.5f, 1.5f);
        coinsLeft = Mathf.FloorToInt(Mathf.Lerp(coinsMin, coinsMax, spawnStrength * coinsAcc / (float)coinsMax));
        coinsAcc -= coinsLeft;
        coinNextZ = coinRunGap;
        if (keepCurrentX) // continue a coin chain
            coinCurrentX = coinTargetX;
        else
            coinCurrentX = Random.Range(challengeXMin, challengeXMax);
        if (Random.Range(0f, 1f) < coinChanceAngled)
        {
            // angle the coins
            bool right;
            float offset = Random.Range(coinAngledXMin, coinAngledXMax);
            if (coinCurrentX + offset > challengeXMax)
                right = false;
            else if (coinCurrentX - offset < challengeXMin)
                right = true;
            else
                right = CoSephUtils.RandomBool();

            if (right)
                coinTargetX = coinCurrentX + offset;
            else
                coinTargetX = coinCurrentX - offset;
        }
        else
        {
            // go in a straight line
            coinTargetX = coinCurrentX;
        }
    }

    // place a single coin in the current coin chain
    private void PlaceCoin()
    {
        Vector3 pos = transform.position + baseRot * new Vector3(coinCurrentX, baseHeight + coinHeight, 0f);
        CollectibleCoin coin = Instantiate(coins[0], pos, baseRot, transform);

        coinsLeft--;
        coinsActive.Add(coin);
        coinNextZ = coinGap;

    }

    private float CalcHazardX(float leftmost, float rightmost)
    {
        float returnX = coinCurrentX;
        bool canJumpLeft = (leftmost - 2 * hazardXGap) > challengeXMin;
        bool canJumpRight = (rightmost + 2 * hazardXGap) < challengeXMax;

        // need to select the position of the next hazard
        if ((!canJumpLeft && !canJumpRight) || CoSephUtils.RandomBool())
        {
            // place the second hazard directly adjacent to the first
            bool right;
            if (rightmost + hazardXGap > challengeXMax)
                right = false;
            else if (leftmost - hazardXGap < challengeXMin)
                right = true;
            else
                right = CoSephUtils.RandomBool();

            if (right)
                returnX = rightmost + hazardXGap;
            else
                returnX = leftmost - hazardXGap;
        }
        else
        {
            // place the next hazard spaced away from the first
            bool right;
            if (!canJumpRight)
                right = false;
            else if (!canJumpLeft)
                right = true;
            else
                right = CoSephUtils.RandomBool();

            if (right)
                returnX = Random.Range(rightmost + hazardXGap * 2f, challengeXMax);
            else
                returnX = Random.Range(challengeXMin, leftmost - hazardXGap * 2f);
        }

        return returnX;
    }

    // place the required number of hazards
    // need to make sure that these are spaced well to allow a gap for the player
    private void PlaceHazard()
    {
        float[] hazardX = new float[hazardsLeft];

        hazardX[0] = coinCurrentX;

        // firstly need to work out the hazard spacing, avoid overlaps and leave the player a gap
        if (hazardsLeft > 1)
            hazardX[1] = CalcHazardX(hazardX[0], hazardX[0]);

        if (hazardsLeft == 3)
            hazardX[2] = CalcHazardX(Mathf.Min(hazardX[0], hazardX[1]), Mathf.Max(hazardX[0], hazardX[1]));

        for (int i = 0; i < hazardsLeft; i++)
        {
            Vector3 pos = transform.position + baseRot * new Vector3(hazardX[i], baseHeight, 0f);
            HazardBase hazard = Instantiate(hazards[0], pos, baseRot, transform);
            hazardsActive.Add(hazard);
            hazardAcc--;
        }
        hazardsLeft = 0;
        coinNextZ = coinRunGap;
    }

    private void Update()
    {
        if (hazardsLeft > 0)
        {
            if (coinNextZ <= 0)
            {
                PlaceHazard();

                // hazardsLeft will always be 0 here, they're always placed at once in a row not in a chain
                if (coinsAcc > 0)
                {
                    // if there are coins built up, always place them after a hazard before more hazards
                    StartCoinChain(true);
                }
                else if (hazardAcc > 0)
                {
                    // should be rare, but if we do get two hazard chains in a row we want to stagger them
                    StartHazardChain(false);
                }
                else
                {
                    // nothing can appear right away so force a gap before the next run of coins to look more structured
                    coinNextZ = coinRunGap;
                }
            }
        }
        else if (coinsLeft > 0)
        {
            if (coinNextZ <= 0)
            {
                // currently in the middle of spawning a coin chain
                PlaceCoin();

                if (coinsLeft > 0)
                {
                    if (!Mathf.Approximately(coinTargetX, coinCurrentX))
                    {
                        // move along the angled coin line
                        coinCurrentX = (coinTargetX - coinCurrentX) / coinsLeft + coinCurrentX;
                    }
                }
                else if (hazardAcc > 0)
                {
                    // if there are hazards built up, always place them after a coin chain before more coins
                    StartHazardChain(true);
                }
                else if (coinsAcc > 0)
                {
                    // start another coin chain immediately following this one, when two coin chains are in sequence let them connect
                    StartCoinChain(true);
                    coinNextZ = coinGap;
                }
                else
                {
                    // nothing can appear right away so force a gap before the next run of coins to look more structured
                    coinNextZ = coinRunGap;
                }
            }
        }
        else
        {
            // not currently spawning coins
            if (coinNextZ < 0)
            {
                if (hazardAcc > 0)
                {
                    StartHazardChain();
                }
                else if (coinsAcc > 0)
                {
                    StartCoinChain();
                }
            }
        }

        if (coinsActive.Count > 0)
        {
            if (coinsActive[0])
            {
                if (coinsActive[0].transform.position.z < challengeZRemoval)
                {
                    coinsActive[0].Remove();
                    coinsActive.RemoveAt(0);
                }
            }
            else
            {
                // TODO do this properly
                // remove a coin that has been deleted
                coinsActive.RemoveAt(0);
            }
        }
        if (hazardsActive.Count > 0)
        {
            if (hazardsActive[0])
            {
                if (hazardsActive[0].transform.position.z < challengeZRemoval)
                {
                    Destroy(hazardsActive[0]);
                    hazardsActive.RemoveAt(0);
                }
            }
            else
            {
                hazardsActive.RemoveAt(0);
            }
        }
    }
}
