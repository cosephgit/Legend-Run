using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UIBaseAccumulator
// a base UI class with methods for displaying a number which ticks gradually towards the current value
// created 18/8/23
// last modified 7/9/23

public enum AccumulatorType
{
    Coins,
    Distance,
    Gems
}

public class UIBaseAccumulator : UIBase
{
    [Header("Accumulator UI settings")]
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private float updateTime = 0.1f; // time between counter updates
    [SerializeField] private AudioClip updatePip;
    [SerializeField] private AudioClip updatePipDown;
    [SerializeField] private UIPopMaker popMaker;
    [SerializeField] private AccumulatorType type = AccumulatorType.Coins;
    [SerializeField] private bool DEBUGPRESENTATIONMODE = false;
    private int valueCurrent;
    private int valueDisplay;
    private float updateNext;

    // show the current value using the format for the accumulator type
    private void DisplayValue()
    {
        switch (type)
        {
            case AccumulatorType.Coins:
                counter.text = GlobalVars.DisplayCoins(valueDisplay);
                break;
            case AccumulatorType.Distance:
                counter.text = GlobalVars.DisplayDistance(valueDisplay);
                break;
            case AccumulatorType.Gems:
                counter.text = GlobalVars.DisplayGems(valueDisplay);
                break;
        }
    }

    // set this display to the new value
    // force means set the display to the new value immediately rather than ticking towards it
    public void SetValue(float amount, bool force = false)
    {
        if (DEBUGPRESENTATIONMODE) return;
        gameObject.SetActive(true);
        valueCurrent = Mathf.FloorToInt(amount);
        if (force)
        {
            valueDisplay = valueCurrent;
            DisplayValue();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (valueCurrent != valueDisplay)
        {
            if (updateNext > 0)
            {
                updateNext -= Time.unscaledDeltaTime;
            }
            else
            {
                float difference = valueCurrent - valueDisplay;
                bool loss = (difference < 0);
                int change = 1;

                if (loss) difference = -difference;

                int exponent = Mathf.FloorToInt(Mathf.Log10(difference));
                int coefficient = Mathf.FloorToInt(difference / Mathf.Pow(10f, exponent));

                if (coefficient > 5)
                    change = 5;

                change = Mathf.FloorToInt(change * Mathf.Pow(10, exponent));

                valueDisplay += (loss ? -1 : 1) * change;

                if (popMaker && change > 0)
                    popMaker.MakePops(exponent);
                    //UIPopManager.instance.ShowPops(popPos.position, popStrength + exponent, popColor);

                AddShake(0.5f);
                if (loss && updatePipDown)
                    AudioManager.instance.SoundPlayEven(updatePipDown, Vector2.zero, 0.3f);
                else
                    AudioManager.instance.SoundPlayEven(updatePip, Vector2.zero, 0.3f);
                DisplayValue();

                updateNext = updateTime;
            }
        }
    }
}
