using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIPopManager
// shows UI pops and sparkles
// created 24/8/23
// last modified 6/9/23

public class UIPopManager : MonoBehaviour
{
    public static UIPopManager instance;
    [SerializeField] private List<UIPop> pops;
    [SerializeField]private int popsCount = 20;
    [SerializeField] private float popSpread = 0.01f; // initial position spread as a fraction of screen width
    [SerializeField] private float popSpeedMin = 0.2f; // as a fraction of screen width
    [SerializeField] private float popSpeedMax = 0.4f;
    [field: SerializeField] public float popGravity { get; private set; } = -1f;
    [SerializeField] private float popsPerMagnitude = 5f;
    [Header("Fadeout")]
    [SerializeField] private float fadeTime = 1f; // how long the pops should fade out over
    public float popScreenScale { get; private set; }
    private List<UIPop> popsActive;

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

        popsActive = new List<UIPop>();

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
            pops[i].Initialise(this);
        }
    }

    private void Start()
    {
        popScreenScale = Screen.width;
    }

    // show pops at the requested point
    public void ShowPops(Vector2 pos, float magnitude, Color color)
    {
        int popsAdd = Mathf.CeilToInt(magnitude * popsPerMagnitude);
        if (popsAdd > pops.Count) popsAdd = pops.Count;

        color.a *= 0.5f;

        for (int i = 0; i < popsAdd; i++)
        {
            UIPop popNew = pops[0];
            Vector2 popPos = pos;
            float angle = Random.Range(0f, 2f);
            Vector2 vee = new Vector2(Mathf.Cos(angle * Mathf.PI), Mathf.Sin(angle * Mathf.PI)) * Random.Range(popSpeedMin, popSpeedMax) * popScreenScale;
            popPos.x -= Mathf.Cos(angle * Mathf.PI) * popSpread * popScreenScale;
            popPos.y -= Mathf.Sin(angle * Mathf.PI) * popSpread * popScreenScale;


            pops.RemoveAt(0);

            popsActive.Add(popNew);

            popNew.Launch(vee, Quaternion.Euler(0f, 0f, angle * 360f), pos, color);
        }
    }

    // a pop has finished being used and is available for use again
    public void RestorePop(UIPop popRestore)
    {
        popsActive.Remove(popRestore);
        pops.Add(popRestore);
    }
}
