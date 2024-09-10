using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIResourceBars
// manages all the main resource bars
// should exist in every scene
// created 12/8/24
// modified 12/8/24

public class UIResourceBars : MonoBehaviour
{
    [SerializeField] private UIResourceBar barCoins;
    [SerializeField] private UIResourceBar barGems;

    public void Initialise()
    {
        UpdateResources();
    }

    public void UpdateResources()
    {
        int coins = GameManager.instance.coinsStash;
        int gems = GameManager.instance.gemsStash;

        if (coins > 0 || GameManager.instance.GetFlag(GlobalVars.SAVEFLAGCOINS))
        {
            barCoins.gameObject.SetActive(true);
            barCoins.SetValue(coins, true);
            GameManager.instance.SetFlag(GlobalVars.SAVEFLAGCOINS);
            //coinsBox.SetActive(true);
            //coinsText.text = GlobalVars.DisplayCoins(coins);
        }
        else
            barCoins.gameObject.SetActive(false);

        if (gems > 0 || GameManager.instance.GetFlag(GlobalVars.SAVEFLAGGEMS))
        {
            barGems.gameObject.SetActive(true);
            barGems.SetValue(gems, true);
            GameManager.instance.SetFlag(GlobalVars.SAVEFLAGGEMS);
        }
        else
            barGems.gameObject.SetActive(false);
    }

    public Transform GetCoinsTransform() { return barCoins.transform; }
    public Transform GetGemsTransform() { return barGems.transform; }
}
