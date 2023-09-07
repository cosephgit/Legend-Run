using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TerrainManager
// this class controls the procedural terrain generation and movement to sustain the player's environment
// created 18/8/23
// last modified 4/9/23

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;
    [SerializeField] private TerrainSegment terrainPrefab;
    [SerializeField] private int terrainMax = 5;
    [SerializeField] private float terrainGap = 20f;
    [SerializeField] private float terrainDisappear = -10f;
    [SerializeField] private float circleRadius = 100f; // the radius of the track circle
    [field: SerializeField] public float moveMinX { get; private set; } = -3f;
    [field: SerializeField] public float moveMaxX { get; private set; } = 3f;
    [Header("Safe route settings")]
    [SerializeField] private float safeRouteZMin = 10f; // minimum distance between safe route direction changes
    [SerializeField] private float safeRouteZMax = 20f; // maximum distance between safe route direction changes
    [SerializeField] private float safeRouteChanceAngled = 0.5f; // the proportion of coin chains that will run at an angle
    [SerializeField] private float safeRouteAngledXMin = 2f; // the minimum x offset for an angled run of coins
    [SerializeField] private float safeRoutePlayerMoveFraction = 0.8f; // the maximum fraction of the player's move speed that an angled route can require
    [Header("Challenge settings")]
    [SerializeField] private TerrainChallenges terrainChallenges;
    [SerializeField] private int terrainChallengeLead = 3;
    [Header("Pause and menu settings")]
    [SerializeField] private UIMenus menuScreens;
    [SerializeField] private UIOdometer odometer;
    [SerializeField] private float pauseOutTime = 1f; // how long to take before resuming full game speed
    [Header("Music")]
    [SerializeField] private AudioClip music;
    [Header("Sound effects")]
    [SerializeField] private AudioClip playerVictory;
    [SerializeField] private AudioClip playerAchievement;
    [SerializeField] private AudioClip playerDefeat;
    [Header("Difficulty")]
    [SerializeField] private float checkpointDistance = 1000f;
    [SerializeField] private float difficultyBase = 0;
    [SerializeField] private float difficultyPerKM = 0.5f;
    private List<TerrainSegment> terrain; // a list of all existing terrain, from furthest back (lowest index) to furthest forward (highest index)
    private float terrainSpeed;
    private float degreesPerSegment; // the number of degrees between each segment
    private float degreesPerSpeed; // the number of degrees to rotate the world for each unit of movement speed
    private Quaternion rotPerSpeed;
    private float pauseResumeTimeLeft;
    public bool paused { get; private set; }
    public bool victory { get; private set; }
    public bool defeat { get; private set; }
    // safe route values
    private float safeCurrentX;
    private float safeTargetX;
    private float safeNextZ; // Z distance before the next change in safe route direction
    // checkpoint scoring
    private float distanceTravelled;
    private float difficulty;

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

        terrain = new List<TerrainSegment>();

        degreesPerSegment = 180f * terrainGap / circleRadius / Mathf.PI;
        degreesPerSpeed = 180f / circleRadius / Mathf.PI;
        rotPerSpeed = Quaternion.Euler(-degreesPerSpeed, 0, 0);
        for (int i = 0; i < terrainMax; i++)
            AddTerrainSegmentCircle();
        //AddTerrainSegment();
        safeCurrentX = Random.Range(moveMinX, moveMaxX);
        distanceTravelled = 0f;

        odometer.gameObject.SetActive(false);
    }

    private void Start()
    {
        difficulty = difficultyBase;
        CalcNextSafeX();
        terrainChallenges.Initialise(circleRadius, Quaternion.Euler(degreesPerSegment * terrainChallengeLead, 0f, 0f));
        terrainChallenges.SetDifficulty(difficulty);
        AudioManager.instance.MusicPlay(music);
    }

    private void CalcNextSafeX()
    {
        safeNextZ = Random.Range(safeRouteZMin, safeRouteZMax);

        if (GameManager.instance.KarmicChance(safeRouteChanceAngled))
        {
            // angle off
            float shiftRightMaxX = Mathf.Min(safeRoutePlayerMoveFraction * safeNextZ / PlayerPawn.instance.PlayerXPerZ(), moveMaxX - safeCurrentX);
            float shiftLeftMaxX = Mathf.Min(safeRoutePlayerMoveFraction * safeNextZ / PlayerPawn.instance.PlayerXPerZ(), safeCurrentX - moveMinX);
            float shift;
            bool right;

            if (shiftRightMaxX < safeRouteAngledXMin)
                right = false;
            else if (shiftLeftMaxX < safeRouteAngledXMin)
                right = true;
            else
                right = (Random.Range(-shiftLeftMaxX, shiftRightMaxX) >= 0);

            if (right) shift = Random.Range(safeRouteAngledXMin, shiftRightMaxX);
            else shift = -Random.Range(safeRouteAngledXMin, shiftLeftMaxX);

            // adjust the karmic balance a little - the bigger the angle shift on the next step, the more of a balance fudge in favour of the player
            GameManager.instance.KarmicAdjust(Mathf.Abs(shift / (moveMaxX - moveMinX) / safeNextZ));

            safeTargetX = safeCurrentX + shift;
        }
        else
        {
            // straight line
            safeTargetX = safeCurrentX;
        }
    }

    // add the next terrain segment at the end of the track (circular track version)
    private void AddTerrainSegmentCircle()
    {
        Vector3 pos;
        Quaternion facing;

        if (terrain.Count > 0)
        {
            float angle = terrain[terrain.Count - 1].transform.eulerAngles.x + degreesPerSegment;
            facing = Quaternion.Euler(angle, 0, 0);
            //pos = facing * Vector3.up * circleRadius + transform.position;

            //pos = terrain[terrain.Count - 1].transform.position;

            //pos.z += terrainGap;
        }
        else
        {
            //pos = transform.position; // the first segment will always be placed above the origin
            //pos.y += circleRadius;
            //facing = Quaternion.identity;
            facing = transform.rotation;
        }

        pos = facing * Vector3.up * circleRadius + transform.position;

        TerrainSegment terrainNew = Instantiate(terrainPrefab, pos, facing, transform);
        terrainNew.Generate();
        terrain.Add(terrainNew);
    }

    // add the next terrain segment at the end of the track
    private void AddTerrainSegment()
    {
        Vector3 pos = transform.position; // the first segment will always be placed at the origin

        if (terrain.Count > 0)
        {
            pos = terrain[terrain.Count - 1].transform.position;

            pos.z += terrainGap;
        }

        TerrainSegment terrainNew = Instantiate(terrainPrefab, pos, transform.rotation, transform);
        terrainNew.Generate();
        terrain.Add(terrainNew);
    }

    public void SetTerrainSpeed(float speed)
    {
        terrainSpeed = speed;
    }

    private void Update()
    {
        if (!paused)
        {
            Vector3 pos = transform.position;
            Vector3 rot = transform.eulerAngles;
            float frameDist = terrainSpeed * Time.deltaTime;
            bool specialSpawn = false;
            float distanceNew = distanceTravelled + frameDist;

            if (PlayerPawn.instance.tutorial.state == TutorialState.Finished)
            {
                if (Mathf.Floor(distanceTravelled / checkpointDistance) < Mathf.Floor(distanceNew / checkpointDistance))
                {
                    // have passed a difficulty milestone
                    difficulty = difficultyBase + (Mathf.Floor(distanceNew / checkpointDistance) * difficultyPerKM);
                    terrainChallenges.SetDifficulty(difficulty);
                    Debug.Log("new difficulty is " + difficulty);
                }

                distanceTravelled = distanceNew;

                odometer.SetDistance(distanceTravelled);
            }

            transform.Rotate(-degreesPerSpeed * frameDist, 0, 0);

            if (!Mathf.Approximately(safeCurrentX, safeTargetX))
                safeCurrentX += (safeTargetX - safeCurrentX) * Mathf.Min(frameDist / safeNextZ, 1f);
            safeNextZ -= frameDist;
            if (safeNextZ <= 0)
            {
                // just hit a "corner" in the safe path, looks good to have powerups spawn here
                specialSpawn = true;
                CalcNextSafeX();
            }

            terrainChallenges.AddDistance(frameDist, safeCurrentX, specialSpawn);

            // remove the nearest terrain segment if it's past out of sight
            if (terrain[0].transform.position.z < terrainDisappear)
            {
                terrain[0].Eliminate();
                terrain.RemoveAt(0);
                //AddTerrainSegment();
                AddTerrainSegmentCircle();
            }
        }

        if (Input.GetButtonDown("Cancel")) PausePressed();
        else if (!paused && pauseResumeTimeLeft > 0)
        {
            pauseResumeTimeLeft -= Time.fixedDeltaTime;
            Time.timeScale = Mathf.Lerp(1f, 0f, pauseResumeTimeLeft / pauseOutTime);
        }
    }

    // a pause button has been pressed
    public void PausePressed()
    {
        if (PlayerPawn.instance.tutorial.state != TutorialState.Finished) return;

        if (defeat) return;
        if (paused)
            UnpauseGame();
        else
            PauseGame();
    }

    // pause the game
    public void PauseGame()
    {
        Time.timeScale = 0f;
        menuScreens.OpenPauseMenu();
        paused = true;
    }

    // unpause the game
    public void UnpauseGame()
    {
        if (victory || defeat) return;

        pauseResumeTimeLeft = pauseOutTime;
        menuScreens.CloseMenu();
        paused = false;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void PlayerVictory()
    {
        AudioManager.instance.MusicPlaySting(playerVictory);
        Time.timeScale = 0f;
        //menuScreens.OpenVictoryMenu(coinsCollected, terrainChallenges.coinsValueTotal, hitsTaken, timeTaken, timePar);
        paused = true;
        victory = true;
    }

    // player has been defeated
    public void PlayerDefeat()
    {
        AudioManager.instance.MusicPlaySting(playerDefeat);
        Time.timeScale = 0f;
        menuScreens.OpenEndingMenu(distanceTravelled, PlayerPawn.instance.pawnPurse.coins);
        paused = true;
        defeat = true;
    }

    // used to notify the terrain manager that a tutorial stage has been completed and adjust the scene accordingly
    public void PlayerTutorialStage()
    {
        if (PlayerPawn.instance.tutorial.state == TutorialState.Finished)
        {
            // tutorial finished, turn on basic UI elements
            odometer.gameObject.SetActive(true);
            menuScreens.StageStart();
        }
        terrainChallenges.ClearChallenges();
    }
}
