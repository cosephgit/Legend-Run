using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIDefeatFirst
// defeat screen for the first defeat
// redirects to shop and some free gems to get started

public class UIDefeatFirst : UIMenuDefeat
{
    [Header("First defeat params")]
    [SerializeField] private int gemsFree = 10;
    [SerializeField] private Button buttonFirstDefeatContinue;
    [SerializeField] private RectTransform buttonFirstDefeatContinueRect;


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

        buttonFirstDefeatContinue.interactable = false;

        PlayerPawn.instance.tutorial.TutorialShopStart(this);
    }

    public void TutorialReady()
    {
        buttonFirstDefeatContinue.interactable = true;
        UIPopManager.instance.StartPopRect(buttonFirstDefeatContinueRect, Color.white, true);
    }

    public void TutorialShopDone()
    {
        menuHub.menuShop.ShopTutorialPrompt();
    }
    public void TutorialShopFinished()
    {
        menuHub.menuShop.ShopTutorialFinished();
    }

    public void ButtonTutorialShop()
    {
        UIPopManager.instance.StopPopRect();
        PlayerPawn.instance.tutorial.TutorialShopOpened();
        menuHub.ButtonShopCoinInGame();
        menuHub.menuShop.OpenShopTutorial();
        gameObject.SetActive(false);
    }
}
