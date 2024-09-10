using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// UIPopManager
// shows UI pops and sparkles
// created 24/8/23
// last modified 24/7/24

public class UIPopManager : MonoBehaviour
{
    public static UIPopManager instance;
    [Header("Pops")]
    [SerializeField] private UIPop popPrefab;
    [SerializeField] private int popsCount = 20;
    [SerializeField] private float popSpread = 0.01f; // initial position spread as a fraction of screen width
    [SerializeField] private float popSpeedMin = 0.2f; // as a fraction of screen width
    [SerializeField] private float popSpeedMax = 0.4f;
    [field: SerializeField] public float popGravity { get; private set; } = -1f;
    [SerializeField] private float popsPerMagnitude = 5f;
    [FormerlySerializedAs("fadeTime")][SerializeField] private float popFadeTime = 2f; // how long the pops should fade out over
    [Header("Sparkles")]
    [SerializeField] private UIPop sparklePrefab;
    [SerializeField] private int sparkleCount = 50;
    [SerializeField] private float sparkleSpeedMin = 0.1f; // as a fraction of screen width
    [SerializeField] private float sparkleSpeedMax = 0.2f;
    [SerializeField] private float sparkleTouchRate = 5f;
    [field: SerializeField] public float sparkleGravity { get; private set; } = 0.25f;
    [SerializeField] private float sparkleFadeTime = 1f; // how long the pops should fade out over
    [Header("PopRect rate")]
    [SerializeField] private float popRectRate = 5; // pops per second in the poprect
    [SerializeField] private bool DEBUGPRESENTATIONMODE = false;
    public float popScreenScale { get; private set; }
    private List<UIPop> popsIdle;
    private List<UIPop> popsActive;
    private List<UIPop> sparkleIdle;
    private List<UIPop> sparkleActive;
    // special effect coroutines
    private Coroutine popRect;
    private Coroutine popTrail;
    // touch sparkles
    private Vector2 touchSparklePos;
    private bool touchSparkle;
    private float touchSparkleNext;

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

        sparkleIdle = new List<UIPop>();
        sparkleActive = new List<UIPop>();

        for (int i = 0; i < sparkleCount; i++)
        {
            sparkleIdle.Add(Instantiate(sparklePrefab, transform));
            sparkleIdle[i].Initialise(this);
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
        if (popsAdd > popsIdle.Count)
        {
            Debug.LogWarning("ran out of pops! wanted " + popsAdd + " got " + popsIdle.Count);
            popsAdd = popsIdle.Count;
        }

        for (int i = 0; i < popsAdd; i++)
            PopRandom(pos, color, PopType.Pop, 1f);
    }

    private void PopRandom(Vector2 pos, Color color, PopType popType, float speed)
    {
        Pop(pos, color, popType, Random.Range(0f, 2f * Mathf.PI), popSpread, speed);
    }

    private void Pop(Vector2 pos, Color color, PopType popType, float angle, float spread, float speed)
    {
        UIPop popNew;
        if (popType == PopType.Pop)
        {
            if (popsIdle.Count == 0)
            {
                Debug.LogWarning("ran out of pops!");
                return;
            }
            popNew = popsIdle[0];
        }
        else
        {
            if (sparkleIdle.Count == 0)
            {
                Debug.LogWarning("ran out of sparkles!");
                return;
            }
            popNew = sparkleIdle[0];
        }

        Vector2 popPos = pos;
        Vector2 vee = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        if (spread > 0)
        {
            popPos.x += Random.value * spread * popScreenScale;
            popPos.y -= Random.value * spread * popScreenScale;
        }

        if (popType == PopType.Pop)
        {
            vee *= Random.Range(popSpeedMin, popSpeedMax);
            vee.y += popSpeedMin; // always bump upwards vee a little
            vee = vee * popScreenScale * speed;
            popsIdle.RemoveAt(0);
            popsActive.Add(popNew);

            popNew.Launch(vee, Quaternion.Euler(0f, 0f, angle * 360f), pos, color, popFadeTime);
        }
        else
        {
            vee *= Random.Range(sparkleSpeedMin, sparkleSpeedMax);
            vee.y -= sparkleSpeedMin; // always bump upwards vee down a little (sparkles float up)
            vee = vee * popScreenScale * speed;
            sparkleIdle.RemoveAt(0);
            sparkleActive.Add(popNew);

            popNew.Launch(vee, Quaternion.Euler(0f, 0f, angle * 360f), pos, color, sparkleFadeTime);
        }
    }

    // a pop has finished being used and is available for use again
    public void RestorePop(UIPop popRestore)
    {
        if (popRestore.popType == PopType.Pop)
        {
            popsActive.Remove(popRestore);
            popsIdle.Add(popRestore);
        }
        else // PopType.Sparkle
        {
            sparkleActive.Remove(popRestore);
            sparkleIdle.Add(popRestore);
        }
    }

    public void StartPopRect(RectTransform rectFill, Color rectColor, bool rectCircle)
    {
        if (popRect != null)
            StopCoroutine(popRect);

        popRect = StartCoroutine(PopRect(rectFill, rectColor, rectCircle));
    }
    public void StopPopRect()
    {
        if (popRect != null)
        {
            StopCoroutine(popRect);
            popRect = null;
        }
    }

    // creates pops in the assigned rect until stopped
    private IEnumerator PopRect(RectTransform rectFill, Color rectColor, bool rectCircle)
    {
        float popDelay = 1f / popRectRate;

        if (rectCircle) popDelay *= 2f;
        while (true)
        {
            Vector2 pos;
            float angle;
            if (rectCircle)
            {
                angle = Random.Range(0, 2f * Mathf.PI);
                pos = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * Random.Range(0f, rectFill.rect.width) * 0.25f;
            }
            else
            {
                pos = new Vector2(Random.Range(rectFill.rect.xMin, rectFill.rect.xMax), Random.Range(rectFill.rect.yMin, rectFill.rect.yMax)) * 0.5f;
                angle = Mathf.Atan2(-pos.x, pos.y);
            }
            pos += (Vector2)rectFill.transform.position;
            Pop(pos, rectColor, PopType.Sparkle, angle, 0f, 1f);
            yield return new WaitForSecondsRealtime(popDelay);
        }
    }

    public void StartPopTrail(Transform start, Transform end, float duration, Color popColor, int popRate)
    {
        if (popTrail != null)
            StopCoroutine(popTrail);

        popTrail = StartCoroutine(PopTrail(start, end, duration, popColor, popRate));
    }
    public void StopPopTrail()
    {
        if (popTrail != null)
        {
            StopCoroutine(popTrail);
            popTrail = null;
        }
    }

    private IEnumerator PopTrail(Transform start, Transform end, float duration, Color popColor, float popRate)
    {
        int popCount = Mathf.CeilToInt(popRate / 20f);
        float timeLeft = duration;
        float popDelay = popCount / popRate;

        while (timeLeft > 0)
        {
            Vector2 pos = Vector2.Lerp(end.position, start.position, timeLeft / duration);
            timeLeft -= popDelay;
            for (int i = 0; i < popCount; i++)
                PopRandom(pos, popColor, PopType.Pop, 1f);
            yield return new WaitForSecondsRealtime(popDelay);
        }
    }

    // generate sparkles at the touch point constantly
    private void LateUpdate()
    {
        if (touchSparkle)
        {
            touchSparkleNext -= Time.deltaTime;
            if (touchSparkleNext < 0)
            {
                touchSparkleNext = 1f / sparkleTouchRate;
                PopRandom(touchSparklePos, Color.white, PopType.Sparkle, 0.1f);
            }
        }
    }

    public void UpdateTouch(Vector2 pos)
    {
        touchSparklePos = pos;
        if (!touchSparkle)
        {
            touchSparkle = true;
            touchSparkleNext = 0f;
        }
    }
    public void EndTouch()
    {
        touchSparkle = false;
    }
}
