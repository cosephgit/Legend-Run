using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// defeat menu when the player quits or loses

public class UIMenuDefeat : MonoBehaviour
{
    [SerializeField] protected UIMenus menuHub;
    [SerializeField] protected UIBaseAccumulator defeatDistance;
    [SerializeField] protected UIBase defeatHighScore;
    [SerializeField] private int defeatPopCount = 4;
    [SerializeField] private float defeatPopMagnitude = 4f;
    [SerializeField] private float defeatPopDelay = 0.5f;
    [SerializeField] protected GameObject buttonContinueAd;
    [SerializeField] protected GameObject buttonContinueGem;
    [SerializeField] private TextMeshProUGUI defeatGemPrice;
    [SerializeField] private GameObject placeholderAdWall;
    [SerializeField] private AudioClip recoverySound;

    public void Initialise()
    {
        gameObject.SetActive(false);
    }

    public virtual void Open(int distance, int coins, int gems)
    {
        int revivecost = GameManager.instance.shopSettings.reviveGemCost;

        gameObject.SetActive(true);

        GameManager.instance.SetCoins(coins);
        GameManager.instance.SetGems(gems);

        if (gems < revivecost)
        {
            buttonContinueAd.gameObject.SetActive(true);
            buttonContinueGem.gameObject.SetActive(false);
        }
        else
        {
            defeatGemPrice.text = revivecost.ToString();
            buttonContinueAd.gameObject.SetActive(false);
            buttonContinueGem.gameObject.SetActive(true);
        }

        if (distance > GameManager.instance.distanceBest)
        {
            defeatDistance.SetValue(distance);
            StartCoroutine(NewHighScorePops());
            defeatHighScore.gameObject.SetActive(true);
        }
        else
        {
            defeatDistance.SetValue(distance, true);
            defeatHighScore.gameObject.SetActive(false);
        }
        GameManager.instance.AddDistance(distance);
        GameManager.instance.SaveSettings();
    }

    protected IEnumerator NewHighScorePops()
    {
        for (int i = 0; i < defeatPopCount; i++)
        {
            defeatHighScore.AddShake(defeatPopMagnitude);
            UIPopManager.instance.ShowPops(defeatHighScore.transform.position, defeatPopMagnitude, Color.magenta);
            yield return new WaitForSeconds(defeatPopDelay);
        }
    }

    public void ButtonContinueAd()
    {
        menuHub.SoundButton();
        placeholderAdWall.gameObject.SetActive(true);
    }
    public void ButtonContinueGems()
    {
        if (GameManager.instance.shopSettings.reviveGemCost > GameManager.instance.gemsStash)
        {
            ButtonContinueAd();
        }
        else
        {
            GameManager.instance.AddGems(-GameManager.instance.shopSettings.reviveGemCost);
            //PlayerPawn.instance.pawnPurse.ChangeBars();
            UIMenus.instance.menuResources.UpdateResources();

            AudioManager.instance.SoundPlayEven(recoverySound, Vector2.zero);
            Continue();
        }
    }
    public void ButtonContinuePostAd()
    {
        menuHub.SoundButton();
        placeholderAdWall.gameObject.SetActive(false);
        Continue();
    }
    private void Continue()
    {
        Debug.Log("EXTRA LIFE! GO!");
        TerrainManager.instance.PlayerRecover();
    }
    public void ButtonRestartShop()
    {
        menuHub.ButtonShopCoinInGame();
        gameObject.SetActive(false);
        if (menuHub.menuResources.healthHearts)
            menuHub.menuResources.healthHearts.gameObject.SetActive(false);
    }
    // restart button - send them to the shop first!
    public void ButtonRestart()
    {
        ButtonRestartShop();
        //menuHub.ButtonRestartConfirm();
    }
    public void ButtonQuit()
    {
        menuHub.ButtonDefeatConfirm();
    }
}
