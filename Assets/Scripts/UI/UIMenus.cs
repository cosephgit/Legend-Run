using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// manages the pause and victory menus
// created 19/8/23
// last modified 5/9/23

public class UIMenus : UIMainMenu
{
    [Header("In-game menu views")]
    [SerializeField] private GameObject menuPauseQuitConfirm;
    [Header("In-game menu general objects")]
    [SerializeField] private GameObject menuUnderlay; // covers the game view while menus are open to make the menu pop more
    [SerializeField] private GameObject menuPause;
    [SerializeField] private UIMenuDefeat menuDefeat;
    [SerializeField] private UIDefeatFirst menuDefeatFirst;
    [Header("Pause references")]
    [SerializeField] private TextMeshProUGUI textPauseDefeatTitle; // text on the title so it can be changed between PAUSED and DEFEAT
    [SerializeField] private Button buttonPause; // the main pause button - so it can be disabled on defeat screen
    [SerializeField] private Button buttonResume; // the menu resume button - so it can be disabled on defeat screen
    [SerializeField] private bool DEBUGPRESENTATIONMODE = false;

    protected override void Initialise()
    {
        base.Initialise();

        menuDefeat.Initialise();
        menuDefeatFirst.Initialise();
        CloseMenu();
        buttonPause.gameObject.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        textPauseDefeatTitle.text = "PAUSE";
        buttonPause.interactable = true;
        buttonResume.interactable = true;
        menuUnderlay.gameObject.SetActive(true);
        menuPause.gameObject.SetActive(true);
        ButtonBack(); // show the default pause menu options
    }

    public void OpenDefeatMenu()
    {
        textPauseDefeatTitle.text = "DEFEAT";
        buttonPause.interactable = false;
        buttonResume.interactable = false;
        menuUnderlay.gameObject.SetActive(true);
        menuPause.gameObject.SetActive(true);
        ButtonBack(); // show the default pause menu options
    }

    public void CloseMenu()
    {
        menuUnderlay.gameObject.SetActive(false);
        menuPause.gameObject.SetActive(false);
        menuDefeat.gameObject.SetActive(false);
        menuDefeatFirst.gameObject.SetActive(false);
        if (GameManager.instance)
            GameManager.instance.SaveSettings();
    }
    // the pause button is available during play as well as when paused, so is a toggle
    public void ButtonPause()
    {
        SoundButton();
        TerrainManager.instance.PausePressed();
    }
    // the resume button only appears on the pause menu so just unpauses
    public void ButtonResume()
    {
        SoundButton();
        TerrainManager.instance.UnpauseGame();
    }
    public override void ButtonBack()
    {
        SoundButton();
        menuMain.gameObject.SetActive(true);
        menuOptions.gameObject.SetActive(false);
        menuPauseQuitConfirm.gameObject.SetActive(false);
        GameManager.instance.SaveSettings();
    }
    public override void ButtonOptions()
    {
        SoundButton();
        menuMain.gameObject.SetActive(false);
        menuOptions.gameObject.SetActive(true);
        menuPauseQuitConfirm.gameObject.SetActive(false);
    }
    public void ButtonRestart()
    {
        SoundButton();
        menuMain.gameObject.SetActive(false);
        menuOptions.gameObject.SetActive(false);
        menuPauseQuitConfirm.gameObject.SetActive(false);
    }
    public void ButtonRestartConfirm()
    {
        SoundButton();
        TerrainManager.instance.RestartStage();
    }
    public override void ButtonQuit()
    {
        SoundButton();
        menuMain.gameObject.SetActive(false);
        menuOptions.gameObject.SetActive(false);
        menuPauseQuitConfirm.gameObject.SetActive(true);
    }
    public void ButtonQuitConfirm()
    {
        SoundButton();
        TerrainManager.instance.PlayerDefeat();
    }

    public void ButtonDefeatConfirm()
    {
        SoundButton();
        TerrainManager.instance.QuitToMenu();
    }

    // open the victory menu and show the player how well they've done
    public void OpenEndingMenu(float distance, int coins, int gems)
    {
        int distanceReached = Mathf.FloorToInt(distance);
        SoundButton();

        menuUnderlay.gameObject.SetActive(true);
        buttonPause.interactable = false;

        Debug.Log("tutorial shop flag? " + (GameManager.instance.GetFlag(GlobalVars.SAVEFLAGTUTORIALSHOP)));
        if (GameManager.instance.GetFlag(GlobalVars.SAVEFLAGTUTORIALSHOP))
            menuDefeat.Open(distanceReached, coins, gems);
        else
            menuDefeatFirst.Open(distanceReached, coins, gems);
    }

    // button for continuing to the next stage after victory
    public void ButtonNextStage()
    {
        SoundButton();
    }

    public void ButtonShopCoinInGame()
    {
        StopButtonParticles();
        menuShop.gameObject.SetActive(true);
        menuShop.ButtonTabItems();
    }
    public void ButtonShopGemInGame()
    {
        StopButtonParticles();
        menuShop.gameObject.SetActive(true);
        menuShop.ButtonTabGems();
    }
    // show the pause button only when the stage starts
    public void StageStart()
    {
        if (DEBUGPRESENTATIONMODE) return;
        buttonPause.gameObject.SetActive(true);
    }

    protected override bool IsMainMenu() { return false; }
}
