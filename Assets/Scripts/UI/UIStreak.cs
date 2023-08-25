using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// UIStreak
// shows the collection streak bar
// created 25/8/23
// last modified 25/8/23

public class UIStreak : UIBase
{
    [SerializeField] private Color fillColor = Color.green;
    [SerializeField] private Color fillColorSuper = Color.cyan;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private Slider streakSlider;
    [SerializeField] private Image streakSlideFill;
    [SerializeField] private Transform popPos; // pops should spawn around this point
    [Header("Streak break")]
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private float failDuration = 4f;
    [Header("strak level settings")]
    [SerializeField] private float streakScalePerLevel = 0.1f;
    [SerializeField] private float streakPopTime = 0.5f;
    [SerializeField] private float streakFailScale = 1.5f;
    [SerializeField] private float streakPopScale = 2f;
    private int streakLevel = 0;
    private float streakFill = 0;

    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }

    // called to update the streak (each time a coin is collected after the first few)
    public void StreakUpdate(int level, float fill)
    {
        if (streakLevel == 0)
        {
            StopAllCoroutines();
            transform.position = posOriginal;
            gameObject.SetActive(true);
            shakeIntensity = 0f;
        }
        if (level > streakLevel)
        {
            Color colorNew = Color.Lerp(fillColor, fillColorSuper, level / 20f);
            streakText.color = colorNew;
            streakSlideFill.color = colorNew;
            StopAllCoroutines();
            StartCoroutine(LevelPop());
            UIPopManager.instance.ShowPops(popPos.position, level / 5f, colorNew);
        }

        streakLevel = level;
        streakText.text = level.ToString();
        streakFill = fill;
        streakSlider.value = fill;
        AddShake(0.5f);
    }

    // called when the player breaks a collection streak (by hitting a hazard or missing a coin)
    public void StreakBreak()
    {
        StopAllCoroutines();
        AddShake(4f);
        StartCoroutine(StreakShatter());
        StartCoroutine(LevelPop());
    }

    private IEnumerator LevelPop()
    {
        float popTime = streakPopTime;
        Vector3 levelScale;

        if (streakLevel == 0)
        {
            levelScale = Vector3.one * streakFailScale; // goes big when you fail
            streakText.fontStyle = FontStyles.Bold;
        }
        else
        {
            levelScale = Vector3.one * (1f + streakLevel * streakScalePerLevel);
            streakText.fontStyle = FontStyles.Normal;
        }

        while (popTime > 0)
        {
            streakText.transform.localScale = levelScale * Mathf.Lerp(1f, streakPopScale, popTime / streakPopTime);
            yield return new WaitForEndOfFrame();
            popTime -= Time.deltaTime;
        }

        streakText.transform.localScale = levelScale;
    }

    private IEnumerator StreakShatter()
    {
        float shatterTime = failDuration;
        float barEmptyRate = streakFill / failDuration;

        streakLevel = 0;

        streakText.text = "X";
        streakText.color = failColor;
        streakSlideFill.color = failColor;

        while (shatterTime > 0)
        {
            streakFill -= barEmptyRate * Time.deltaTime;
            streakSlider.value = streakFill;
            shatterTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}
