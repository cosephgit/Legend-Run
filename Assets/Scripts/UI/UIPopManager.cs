using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopManager : MonoBehaviour
{
    public static UIPopManager instance;
    [SerializeField] private List<Image> pops;
    [SerializeField]private int popsCount = 20;
    [SerializeField] private float popSpread = 0.01f; // initial position spread as a fraction of screen width
    [SerializeField] private float popSpeedMin = 0.2f; // as a fraction of screen width
    [SerializeField] private float popSpeedMax = 0.4f;
    [SerializeField] private float popGravity = -1f;
    [SerializeField] private float popsPerMagnitude = 5f;
    private float popScreenScale;
    private List<Image> popsActive;
    private List<Vector2> popsSpeed;

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
        else instance = this;

        popsActive = new List<Image>();
        popsSpeed = new List<Vector2>();

        if (popsCount > pops.Count)
        {
            int popsAdd = popsCount - pops.Count;
            for (int i = 0; i < popsAdd; i++)
            {
                pops.Add(Instantiate(pops[0], transform));
            }
        }

        for (int i = 0; i < pops.Count; i++)
        {
            pops[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        popScreenScale = Screen.width;
    }

    private void Update()
    {
        if (popsActive.Count > 0)
        {
            // move pops
            for (int i = popsActive.Count - 1; i >= 0; i--)
            {
                Vector2 pos = popsActive[i].transform.position;

                pos += popsSpeed[i] * Time.deltaTime;

                popsActive[i].transform.position = pos;

                if (pos.y < -100f)
                {
                    pops.Add(popsActive[i]);
                    popsActive[i].gameObject.SetActive(false);
                    popsActive.RemoveAt(i);
                    popsSpeed.RemoveAt(i);
                }
                else
                {
                    Quaternion rot = popsActive[i].transform.rotation * Quaternion.Euler(0f, 0f, popsSpeed[i].x);
                    Vector2 vee = popsSpeed[i];

                    vee.y += popGravity * popScreenScale * Time.deltaTime;

                    popsSpeed[i] = vee;
                    popsActive[i].transform.rotation = rot;
                }
            }
        }
    }

    // show pops at the requested point
    public void ShowPops(Vector2 pos, float magnitude, Color color)
    {
        int popsAdd = Mathf.CeilToInt(magnitude * popsPerMagnitude);
        if (popsAdd > pops.Count) popsAdd = pops.Count;

        color.a *= 0.5f;

        for (int i = 0; i < popsAdd; i++)
        {
            Image popNew = pops[0];
            Vector2 popPos = pos;
            float angle = Random.Range(0f, 2f);
            Vector2 vee = new Vector2(Mathf.Cos(angle * Mathf.PI), Mathf.Sin(angle * Mathf.PI)) * Random.Range(popSpeedMin, popSpeedMax) * popScreenScale;
            popPos.x -= Mathf.Cos(angle * Mathf.PI) * popSpread * popScreenScale;
            popPos.y -= Mathf.Sin(angle * Mathf.PI) * popSpread * popScreenScale;


            pops.RemoveAt(0);

            popsActive.Add(popNew);
            popsSpeed.Add(vee);

            popNew.transform.rotation = Quaternion.Euler(0f, 0f, angle * 360f); // yes this is intentionally not 180
            popNew.transform.position = pos;
            popNew.color = color;

            if (popsActive.Count != popsSpeed.Count) Debug.LogError("ERROR UIPopManager is out of sync!");

            popNew.gameObject.SetActive(true);
        }
    }
}
