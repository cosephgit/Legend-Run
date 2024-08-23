using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIDefeatFirst
// defeat screen for the first defeat
// redirects to shop and some free gems to get started

public class UIDefeatFirst : UIMenuDefeat
{
    [Header("First defeat params")]
    [SerializeField] private int gemsFree = 10;


    public override void Open(int distance, int coins, int gems)
    {
        gameObject.SetActive(true);

        GameManager.instance.SetCoins(coins);
        GameManager.instance.SetGems(gems);

        buttonContinueAd.gameObject.SetActive(false);
        buttonContinueGem.gameObject.SetActive(false);

        defeatDistance.SetValue(distance);
        StartCoroutine(NewHighScorePops());
        defeatHighScore.gameObject.SetActive(true);

        GameManager.instance.AddDistance(distance);
        GameManager.instance.SaveSettings();

        PlayerPawn.instance.tutorial.TutorialShopStart();
    }


    public void ButtonTutorialShop()
    {

    }
}
