using System.Collections;
using System.Collections.Generic;
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

    [Header("Main menu references")]
    [SerializeField] protected GameObject menuMain;
    [SerializeField] private RectTransform menuPlayRect;
    [SerializeField] private RectTransform menuShopRect;
    [Header("General menu references")]
    [SerializeField] protected UIOptionsMenu menuOptions;
    [SerializeField] protected UIShop menuShop;
    [SerializeField] protected UIMenuMainScores menuScores;
    [SerializeField] protected UIResourceBars menuResources;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private GameObject buttonQuitGame;
    private bool audioReady;
    private State state;

    private void Awake()
    {
        ButtonBack();
#if UNITY_WEBGL
        buttonQuitGame.SetActive(false);
#endif
    }

    private void Start()
    {
        if (menuMusic)
            AudioManager.instance.MusicPlay(menuMusic);
        menuOptions.Initialise(this);
        menuShop.Initialise(this);
        menuOptions.gameObject.SetActive(false);
        menuShop.gameObject.SetActive(false);
        menuResources.Initialise();
        if (IsMainMenu())
        {
            menuScores.Initialise();
            UpdateButtonParticles(menuShop.IsBuyAvailable());
        }
        audioReady = true;
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
        SceneManager.LoadScene(1);
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
