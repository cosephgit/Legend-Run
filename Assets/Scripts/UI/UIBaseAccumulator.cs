using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// UIBaseAccumulator
// a base UI class with methods for displaying a number which ticks gradually towards the current value
// created 18/8/23
// last modified 4/9/23

public enum AccumulatorType
{
    Coins,
    Distance
}

public class UIBaseAccumulator : UIBase
{
    [Header("Accumulator UI settings")]
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private float updateTime = 0.1f; // time between counter updates
    [SerializeField] private AudioClip updatePip;
    [SerializeField] private Transform popPos; // if this UI uses pops, they should spawn around this point
    [SerializeField] private AccumulatorType type = AccumulatorType.Coins;
    private int valueCurrent;
    private int valueDisplay;
    private float updateNext;

    private void DisplayValue()
    {
        switch (type)
        {
            case AccumulatorType.Coins:
                counter.text = GameManager.instance.DisplayCoins(valueDisplay);
                break;
            case AccumulatorType.Distance:
                counter.text = GameManager.instance.DisplayDistance(valueDisplay);
                break;
        }
    }

    public void SetValue(float amount, bool force = false)
    {
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

                if (change > 0)
                    UIPopManager.instance.ShowPops(popPos.position, 1 + exponent, Color.yellow);

                AddShake(0.5f);
                AudioManager.instance.SoundPlayEven(updatePip, Vector2.zero, 0.3f);
                DisplayValue();

                updateNext = updateTime;
            }
        }
    }
}
