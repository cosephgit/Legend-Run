using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIPopManager
// shows UI pops and sparkles
// created 24/8/23
// last modified 24/7/24

public class UIPopManager : MonoBehaviour
{
    public static UIPopManager instance;
    [SerializeField] private UIPop popPrefab;
    [SerializeField]private int popsCount = 20;
    [SerializeField] private float popSpread = 0.01f; // initial position spread as a fraction of screen width
    [SerializeField] private float popSpeedMin = 0.2f; // as a fraction of screen width
    [SerializeField] private float popSpeedMax = 0.4f;
    [field: SerializeField] public float popGravity { get; private set; } = -1f;
    [SerializeField] private float popsPerMagnitude = 5f;
    [Header("Fadeout")]
    [SerializeField] private float fadeTime = 2f; // how long the pops should fade out over
    [SerializeField] private bool DEBUGPRESENTATIONMODE = false;
    public float popScreenScale { get; private set; }
    private List<UIPop> popsIdle;
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

        popsIdle = new List<UIPop>();
        popsActive = new List<UIPop>();

        for (int i = 0; i < popsCount; i++)
        {
            popsIdle.Add(Instantiate(popPrefab, transform));
            popsIdle[i].Initialise(this);
        }
    }

    private void Start()
    {
        popScreenScale = Screen.width;
    }

    // show pops at the requested point
    public void ShowPops(Vector2 pos, float magnitude, Color color)
    {
        if (DEBUGPRESENTATIONMODE) return;

        int popsAdd = Mathf.CeilToInt(magnitude * popsPerMagnitude);
        if (popsAdd > popsIdle.Count) popsAdd = popsIdle.Count;

        for (int i = 0; i < popsAdd; i++)
        {
            UIPop popNew = popsIdle[0];
            Vector2 popPos = pos;
            float angle = Random.Range(0f, 2f);
            float angleRad = angle * Mathf.PI;
            Vector2 vee = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * Random.Range(popSpeedMin, popSpeedMax);
            vee.y += popSpeedMin; // always bump upwards vee a little
            vee = vee * popScreenScale;
            popPos.x -= Mathf.Cos(angleRad) * popSpread * popScreenScale;
            popPos.y -= Mathf.Sin(angleRad) * popSpread * popScreenScale;

            popsIdle.RemoveAt(0);

            popsActive.Add(popNew);

            popNew.Launch(vee, Quaternion.Euler(0f, 0f, angle * 360f), pos, color, fadeTime);
        }
    }

    // a pop has finished being used and is available for use again
    public void RestorePop(UIPop popRestore)
    {
        popsActive.Remove(popRestore);
        popsIdle.Add(popRestore);
    }
}
