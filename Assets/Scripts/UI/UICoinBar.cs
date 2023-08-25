using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// shows the current coin count of the player
// created 22/8/23
// last modified 23/8/23

public class UICoinBar : UIBase
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private float updateTime = 0.1f; // time between counter updates
    [SerializeField] private AudioClip updatePip;
    [SerializeField] private Transform popPos; // if this UI uses pops, they should spawn around this point
    private int coinsCurrent;
    private int coinsDisplay;
    private float updateNext;
    
    private void DisplayCoins()
    {
        if (coinsDisplay > 999999)
            counter.text = coinsDisplay.ToString("E2");
        else
            counter.text = coinsDisplay.ToString("N0");
    }

    public void SetCoins(int amount, bool force = false)
    {
        coinsCurrent = amount;
        if (force) DisplayCoins();
    }

    protected override void Update()
    {
        base.Update();
        if (coinsCurrent != coinsDisplay)
        {
            if (updateNext > 0)
            {
                updateNext -= Time.deltaTime;
            }
            else
            {
                int difference = coinsCurrent - coinsDisplay;
                bool loss = (difference < 0);
                int change = 1;

                if (loss) difference = -difference;

                int exponent = Mathf.FloorToInt(Mathf.Log10(difference));
                int coefficient = Mathf.FloorToInt(difference / Mathf.Pow(10f, exponent));

                if (coefficient > 5)
                    change = 5;

                change = Mathf.FloorToInt(change * Mathf.Pow(10, exponent));

                coinsDisplay += (loss ? -1 : 1) * change;

                if (change > 0)
                    UIPopManager.instance.ShowPops(popPos.position, 0.1f * change, Color.yellow);

                AddShake(0.5f);
                AudioManager.instance.SoundPlayEven(updatePip, Vector2.zero, 0.3f);
                DisplayCoins();

                updateNext = updateTime;
            }
        }
    }
}
