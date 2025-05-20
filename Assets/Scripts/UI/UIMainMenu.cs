using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// general methods used by all menus
// created 31/8/23
// last modified 31/8/23

public class UIMainMenu : MonoBehaviour
{
    private enum State
    {
        Main,
        Options,
        Shop
    }

    public static UIMainMenu instance;
    [Header("Main menu references")]
    [SerializeField] protected GameObject menuMain;
    [SerializeField] private RectTransform menuPlayRect;
    [SerializeField] private RectTransform menuShopRect;
    [SerializeField] private TextMeshProUGUI textVersion;
    [SerializeField] private TextMeshProUGUI textMode;
    [SerializeField] private TextMeshProUGUI googleMode;
    [Header("General menu references")]
    [SerializeField] protected UIOptionsMenu menuOptions;
    [field: SerializeField] public UIShop menuShop { get; private set; }
    [SerializeField] protected UIMenuMainScores menuScores;
    [field: SerializeField] public UIResourceBars menuResources { get; private set; }
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private GameObject buttonDebugReset;
    [SerializeField] private GameObject buttonQuitGame;
    [Header("Rewards")]
    [SerializeField] private UIRewardButton buttonAd;
    [SerializeField] private UIRewardButton buttonFb;
    [Header("Ad providers")]
    [SerializeField] private AdMenuBanner menuBanner;
    private bool audioReady;
    private State state;

    protected void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
            instance = this;

        if (IsMainMenu())
            googleMode.text = "Logging in...";
    }

    private void Start()
    {
        Initialise();
    }

    protected virtual void Initialise()
    {
        if (menuMusic)
            AudioManager.instance.MusicPlay(menuMusic);
        menuOptions.Initialise(this);
        menuShop.Initialise(this);
        menuOptions.gameObject.SetActive(false);
        menuShop.gameObject.SetActive(false);
        Debug.Log("UIMainMenu Start() running - about to initialise resource bars");
        menuResources.Initialise();
        Debug.Log("UIMainMenu Start() running - just initialised resource bars");
        if (IsMainMenu())
        {
            if (buttonDebugReset)
            {
                if (GameManager.instance.mode.debugMode)
                    buttonDebugReset.SetActive(true);
                else
                    buttonDebugReset.SetActive(false);
            }

            menuScores.Initialise();

            textVersion.text = GlobalVars.GAMEVERSIONNAME + Application.version;
            if (GameManager.instance.mode.modeSubtitle.Length > 0)
            {
                textMode.gameObject.SetActive(true);
                textMode.text = GameManager.instance.mode.modeSubtitle;
            }
            else
                textMode.gameObject.SetActive(false);

            UpdateButtonParticles(menuShop.IsBuyAvailable());
        }
        ButtonBack();
#if UNITY_WEBGL
        buttonQuitGame.SetActive(false);
#endif
        audioReady = true;
    }

    // set the Google Play login details
    public void LoginComplete(bool success)
    {
        Debug.Log("UIMainMenu.LoginComplete: " + success);
        menuBanner.Initialise(success);
        if (IsMainMenu())
            googleMode.text = "Logged in? " + success;
    }

    public void SoundButton()
    {
        if (audioReady)
        {
            AudioManager.instance.SoundPlayMenuButton();
        }
    }

    public void ButtonPlay()
    {
        SoundButton();
        SceneManager.LoadScene(GlobalVars.SCENEPLAY);
    }
    public virtual void ButtonOptions()
    {
        SoundButton();
        StopButtonParticles();
        menuMain.SetActive(false);
        menuOptions.gameObject.SetActive(true);
        menuShop.gameObject.SetActive(false);
    }
    public virtual void ButtonShop()
    {
        // SoundButton(); // handled by shop tab button below
        StopButtonParticles();
        menuMain.SetActive(false);
        menuOptions.gameObject.SetActive(false);
        menuShop.gameObject.SetActive(true);
        menuShop.ButtonTabItems();
    }
    public virtual void ButtonShopGems()
    {
        // SoundButton(); // handled by shop tab button below
        StopButtonParticles();
        menuMain.SetActive(false);
        menuOptions.gameObject.SetActive(false);
        menuShop.gameObject.SetActive(true);
        menuShop.ButtonTabGems();
    }
    public virtual void ButtonQuit()
    {
        SoundButton();
        Application.Quit();
        Debug.Log("quit");
    }
    public virtual void ButtonBack()
    {
        SoundButton();
        UpdateButtonParticles(menuShop.IsBuyAvailable());
        menuMain.SetActive(true);
        menuOptions.gameObject.SetActive(false);
        menuShop.gameObject.SetActive(false);
        UpdateRewardButtons();
    }

    public void ButtonAdReward()
    {
        SoundButton();
        GameManager.instance.saveData.lastAdRewards = DateTime.Now;
        Debug.Log("Insert watching ad!");
        UpdateRewardButtons();
    }

    public void ButtonFbReward()
    {
        SoundButton();
        GameManager.instance.saveData.lastFbRewards = DateTime.Now;
        Debug.Log("Insert sharing to facebook!");
        UpdateRewardButtons();
    }

    private void UpdateRewardButtons()
    {
        int delayNeeded;
        double secondsSinceAdBonus = (DateTime.Now - GameManager.instance.saveData.lastAdRewards).TotalSeconds;
        double secondsSinceFbBonus = (DateTime.Now - GameManager.instance.saveData.lastFbRewards).TotalSeconds;
        Debug.Log("it's been " + secondsSinceAdBonus + " seconds since the last ad rewards and " + secondsSinceFbBonus + " seconds since the last fb rewards");
        if (GameManager.instance.mode.debugMode)
            delayNeeded = GlobalVars.DAILYDELAYDEBUG;
        else
            delayNeeded = GlobalVars.DAILYDELAY;

        if (secondsSinceAdBonus > delayNeeded)
            buttonAd.SetEnabled();
        else
            buttonAd.SetDisabled();

        if (secondsSinceFbBonus > delayNeeded)
            buttonFb.SetEnabled();
        else
            buttonFb.SetDisabled();
    }

    // DO NOT SHIP
    public void ButtonDEBUGWIPE()
    {
        if (GameManager.instance.mode.debugMode)
        {
            GameManager.instance.DEBUGWIPEDATA(false);
            UpdateScores();
            UpdateButtonParticles(menuShop.IsBuyAvailable());
        }
    }

    public void UpdateScores()
    {
        menuScores.UpdateScores();
        menuResources.UpdateResources();
    }

    public void UpdateButtonParticles(bool buyOption)
    {
        if (buyOption)
        {
            UIPopManager.instance.StartPopRect(menuShopRect, Color.yellow, true);
        }
        else
        {
            UIPopManager.instance.StartPopRect(menuPlayRect, Color.yellow, false);
        }
    }
    public void StopButtonParticles()
    {
        UIPopManager.instance.StopPopRect();
    }

    protected virtual bool IsMainMenu() { return true; }
}
