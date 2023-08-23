using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TerrainManager
// this class controls the procedural terrain generation and movement to sustain the player's environment
// created 18/8/23
// last modified 18/8/23

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private TerrainSegment terrainPrefab;
    [SerializeField] private TerrainChallenges terrainChallenges;
    [SerializeField] private int terrainMax = 5;
    [SerializeField] private float terrainGap = 20f;
    [SerializeField] private float terrainDisappear = -10f;
    [SerializeField] private float circleRadius = 100f; // the radius of the track circle
    [Header("Music")]
    [SerializeField] private AudioClip music;
    private List<TerrainSegment> terrain; // a list of all existing terrain, from furthest back (lowest index) to furthest forward (highest index)
    private float terrainSpeed;
    private float degreesPerSegment; // the number of degrees between each segment
    private float degreesPerSpeed; // the number of degrees to rotate the world for each unit of movement speed
    private Quaternion rotPerSpeed;

    private void Awake()
    {
        terrain = new List<TerrainSegment>();

        degreesPerSegment = 180f * terrainGap / circleRadius / Mathf.PI;
        degreesPerSpeed = 180f / circleRadius / Mathf.PI;
        rotPerSpeed = Quaternion.Euler(-degreesPerSpeed, 0, 0);
        for (int i = 0; i < terrainMax; i++)
            AddTerrainSegmentCircle();
        //AddTerrainSegment();
    }

    private void Start()
    {
        terrainChallenges.Initialise(circleRadius, Quaternion.Euler(degreesPerSegment * terrainMax, 0f, 0f));
        AudioManager.instance.MusicPlay(music);
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
            pos = facing * Vector3.up * circleRadius + transform.position;

            //pos = terrain[terrain.Count - 1].transform.position;

            //pos.z += terrainGap;
        }
        else
        {
            pos = transform.position; // the first segment will always be placed above the origin
            pos.y += circleRadius;
            facing = Quaternion.identity;
        }

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
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        float frameDist = terrainSpeed * Time.deltaTime;
        transform.Rotate(-degreesPerSpeed * frameDist, 0, 0);
        terrainChallenges.AddDistance(frameDist);
        //pos.z -= terrainSpeed * Time.deltaTime;
        //rot.x -= degreesPerSpeed * terrainSpeed * Time.deltaTime;

        //transform.position = pos;
        //transform.rotation = Quaternion.Euler(rot);

        if (terrain[0].transform.position.z < terrainDisappear)
        {
            terrain[0].Eliminate();
            terrain.RemoveAt(0);
            //AddTerrainSegment();
            AddTerrainSegmentCircle();
        }
    }
}
