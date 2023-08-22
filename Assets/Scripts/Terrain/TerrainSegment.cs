using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TerrainSegment
// this is one segment of the terrain, it is placed by the TerrainManager when needed and then generates features on the segment
// created 18/8/23
// last modified 19/8/23

public class TerrainSegment : MonoBehaviour
{
    [SerializeField] private GameObject[] groundOptions; // sections of terrain that can be used for the ground
    [SerializeField] private float decorZMin = -10f; // all decor positions will be bounded within this Y min and max
    [SerializeField] private float decorZMax = 10f;
    [SerializeField] private GameObject[] scatterOptions; // items that can be scattered around anywhere
    [SerializeField] private int scatterMin = 10; // minimum and maximum scatter per segment
    [SerializeField] private int scatterMax = 20;
    [SerializeField] private float scatterXMin = -10f; // all decor positions will be bounded within this Y min and max
    [SerializeField] private float scatterXMax = 10f;
    [SerializeField] private float scatterY = 0.5f;
    [SerializeField] private GameObject[] borderOptions; // trees or other objects that can be arrayed along the edge of the terrain
    [SerializeField] private int borderMin = 3; // minimum and maximum border per segment EACH SIDE
    [SerializeField] private int borderMax = 6;
    [SerializeField] private float borderXMin = 5f; // the minimum and maximum X values of border objects
    [SerializeField] private float borderXMax = 10f; // (mirrored for left side)
    [SerializeField] private GameObject[] wallOptions; // hills or other blocking objects that can be used to block off the horizon
    [SerializeField] private float wallY = -5f;
    [SerializeField] private float wallZSpaceMin = 0f; // the minimum and maximum GAP between each wall object
    [SerializeField] private float wallZSpaceMax = 5f;
    [SerializeField] private float wallXMin = 10f; // the minimum and maximum X values of border objects
    [SerializeField] private float wallXMax = 15f; // (mirrored for left side)
    // generate the terrain for this segment - call right after instantiation
    public void Generate()
    {
        Vector3 pos = Vector3.zero;
        GameObject decor;

        // place ground
        Instantiate(groundOptions[Random.Range(0, groundOptions.Length)], transform.position, transform.rotation, transform);

        pos.y = scatterY;

        // place scatter
        for (int i = 0; i < Random.Range(scatterMin, scatterMax); i++)
        {
            pos.x = Random.Range(scatterXMin, scatterXMax);
            pos.z = Random.Range(decorZMin, decorZMax);
            decor = Instantiate(scatterOptions[Random.Range(0, scatterOptions.Length)], transform);
            decor.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            decor.transform.localPosition = pos;
        }

        // place right border
        for (int i = 0; i < Random.Range(borderMin, borderMax); i++)
        {
            pos.x = Random.Range(borderXMin, borderXMax);
            pos.z = Random.Range(decorZMin, decorZMax);
            decor = Instantiate(borderOptions[Random.Range(0, borderOptions.Length)], transform);
            decor.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            decor.transform.localPosition = pos;
        }

        // place left border
        for (int i = 0; i < Random.Range(borderMin, borderMax); i++)
        {
            pos.x = -Random.Range(borderXMin, borderXMax);
            pos.z = Random.Range(decorZMin, decorZMax);
            decor = Instantiate(borderOptions[Random.Range(0, borderOptions.Length)], transform);
            decor.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            decor.transform.localPosition = pos;
        }

        pos.y = wallY;
        // place right wall
        pos.z = decorZMin + Random.Range(wallZSpaceMin, wallZSpaceMax);
        pos.x = Random.Range(wallXMin, wallXMax);
        decor = Instantiate(wallOptions[Random.Range(0, wallOptions.Length)], transform);
        decor.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        decor.transform.localPosition = pos;

        // place left wall
        pos.z = decorZMin + Random.Range(wallZSpaceMin, wallZSpaceMax);
        pos.x = -Random.Range(wallXMin, wallXMax);
        decor = Instantiate(wallOptions[Random.Range(0, wallOptions.Length)], transform);
        decor.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        decor.transform.localPosition = pos;
    }

    // remove this terrain segment
    public void Eliminate()
    {
        Destroy(gameObject);
    }
}
