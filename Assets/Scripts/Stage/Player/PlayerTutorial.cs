using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// PlayerTutorial
// manages the tutorial screens the first time the player plays the game
// created 6/9/23
// modified 24/8/24


/*
 * so working out the tutorial...
 * at the start of the level, tell the player how to move (this is an autopass as soon as the player taps)
 * then spawn some coins and tell the player to move to grab them
 * then "well done" after they grab some of them (not all)
 * then spawn a hazard and tell the player to dodge them
 * then "well done" when they dodge successfully
 * then spawn a hazard and tell the player to jump it (lock their position into the hazard line)
 * then "well done" when they jump when the hazard passes
 */

public enum TutorialState
{
    Init,
    Move,
    MoveDone,
    Collect,
    CollectDone,
    Dodge,
    DodgeDone,
    Jump,
    JumpDone,
    JumpOver,
    JumpOverDone,
    Finished
}


public class PlayerTutorial : MonoBehaviour
{
    [Header("Tutorial UI parameters")]
    [SerializeField] private float tutorialResultTime = 2f;
    [Header("Tutorial UI elements")]
    [SerializeField] private UICompanion companionControls;
    [SerializeField] private GameObject tutorialCompanion;
    [SerializeField] private GameObject tutorialCompanionTipBackground;
    [SerializeField] private TextMeshProUGUI tutorialCompanionTip;
    [SerializeField] private UITutorialTip swipeSide;
    [SerializeField] private UITutorialTip swipeUp;
    [SerializeField] private UITutorialFinger tutorialFinger;
    [Header("Success effects")]
    [SerializeField] private Color successPopColor = Color.green;
    [SerializeField] private float successPopIntensity = 4f;
    [SerializeField] private Color minorPopColor = Color.yellow;
    [SerializeField] private float minorPopIntensity = 2f;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip progressSound;
    [Header("Failure effects")]
    [SerializeField] private AudioClip failureSound;
    [Header("Tutorial Shop params")]
    [SerializeField] private float tutorialShopDelayMin = 0.5f;
    [SerializeField] private int tutorialFreeGems = 10;
    [SerializeField] private float tutorialShopDelay = 3f;
    public TutorialState state { get; private set; } = TutorialState.Init;
    private float tutorialDelay;
    private bool tutorialShopWait = true;

    public void Initialise()
    {
        tutorialFinger.Hide();
        if (GameManager.instance.tutorialDone)
        {
            companionControls.SetInactive();
            state = TutorialState.Finished;
            TerrainManager.instance.PlayerTutorialStage();
        }
        else
        {
            state = TutorialState.Move;
            companionControls.ShowCompanion();
            CompanionText("Swipe to move");
            PlayerPawn.instance.TutorialStart();
            tutorialFinger.ShowSwipe(swipeSide);
        }
    }

    // called when the player moves at all
    public void PlayerMoved()
    {
        if (state == TutorialState.Move)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    // called when the player collects a coin
    public void CoinCollected()
    {
        if (state == TutorialState.Collect)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    public void CoinMissed()
    {
        if (state == TutorialState.Collect)
        {
            StartCoroutine(TutorialFailed());
        }
    }


    // called when a hazard is destroyed without hitting the player
    public void HazardDodged()
    {
        if (state == TutorialState.Dodge)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    public void PlayerJumped()
    {
        if (state == TutorialState.Jump)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    // called when the player lands after a hazard is destroyed without hitting the player
    public void HazardJumped()
    {
        if (state == TutorialState.JumpOver)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    // called when the player hits a hazard
    public void HazardHit()
    {
        if (state == TutorialState.Dodge || state == TutorialState.JumpOver)
        {
            StartCoroutine(TutorialFailed());
        }
    }

    // called when a tutorial stage is passed
    private IEnumerator TutorialPassed()
    {
        float stepTime;

        if (GameManager.instance.mode.tutorialFast)
            stepTime = 0.1f;
        else
            stepTime = tutorialResultTime;

        tutorialFinger.Hide();
        switch (state)
        {
            case TutorialState.Move:
                state = TutorialState.MoveDone;
                break;
            case TutorialState.Collect:
                state = TutorialState.CollectDone;
                break;
            case TutorialState.Dodge:
                state = TutorialState.DodgeDone;
                break;
            case TutorialState.Jump:
                state = TutorialState.JumpDone;
                break;
            case TutorialState.JumpOver:
                state = TutorialState.JumpOverDone;
                break;
        }

        companionControls.CompanionHappy(stepTime);

        CompanionText("Well done!", successSound, 2);
        yield return new WaitForSeconds(stepTime);

        switch (state)
        {
            case TutorialState.MoveDone:
                CompanionText("Collect Coins", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                tutorialFinger.ShowSwipe(swipeSide);
                state = TutorialState.Collect;
                break;
            case TutorialState.CollectDone:
                CompanionText("Dodge Hazards", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                tutorialFinger.ShowSwipe(swipeSide);
                state = TutorialState.Dodge;
                break;
            case TutorialState.DodgeDone:
                CompanionText("Swipe up to jump", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                tutorialFinger.ShowSwipe(swipeUp);
                state = TutorialState.Jump;
                break;
            case TutorialState.JumpDone:
                CompanionText("Jump hazards", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                tutorialFinger.ShowSwipe(swipeUp);
                state = TutorialState.JumpOver;
                break;
            case TutorialState.JumpOverDone:
                CompanionText("Great job!", successSound, 2);
                companionControls.CompanionCheer(stepTime);
                yield return new WaitForSeconds(stepTime);
                CompanionText("");
                yield return new WaitForSeconds(stepTime);
                CompanionText("Get ready!", successSound, 2);
                companionControls.CompanionTalk(stepTime);
                yield return new WaitForSeconds(stepTime);
                CompanionText("");
                companionControls.HideCompanion(stepTime);
                state = TutorialState.Finished;
                GameManager.instance.TutorialComplete();
                break;
        }
        TerrainManager.instance.PlayerTutorialStage();
    }

    private IEnumerator TutorialFailed()
    {
        float stepTime;

        if (GameManager.instance.mode.tutorialFast)
            stepTime = 0.1f;
        else
            stepTime = tutorialResultTime;

        tutorialFinger.Hide();
        switch (state)
        {
            case TutorialState.Move:
                state = TutorialState.MoveDone;
                break;
            case TutorialState.Collect:
                state = TutorialState.CollectDone;
                break;
            case TutorialState.Dodge:
                state = TutorialState.DodgeDone;
                break;
            case TutorialState.Jump:
                state = TutorialState.JumpDone;
                break;
            case TutorialState.JumpOver:
                state = TutorialState.JumpOverDone;
                break;
        }

        CompanionText("Try again!", failureSound, 1);
        companionControls.CompanionSad(stepTime);
        yield return new WaitForSeconds(stepTime);

        switch (state)
        {
            case TutorialState.MoveDone:
                CompanionText("Swipe to move");
                companionControls.CompanionTalk(stepTime);
                state = TutorialState.Move;
                tutorialFinger.ShowSwipe(swipeSide);
                break;
            case TutorialState.CollectDone:
                CompanionText("Collect Coins", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                state = TutorialState.Collect;
                tutorialFinger.ShowSwipe(swipeSide);
                break;
            case TutorialState.DodgeDone:
                CompanionText("Dodge hazards", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                state = TutorialState.Dodge;
                tutorialFinger.ShowSwipe(swipeSide);
                break;
            case TutorialState.JumpDone:
                CompanionText("Swipe up to jump", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                state = TutorialState.Jump;
                tutorialFinger.ShowSwipe(swipeUp);
                break;
            case TutorialState.JumpOverDone:
                CompanionText("Jump hazards", progressSound, 1);
                companionControls.CompanionTalk(stepTime);
                state = TutorialState.JumpOver;
                tutorialFinger.ShowSwipe(swipeUp);
                break;
        }

        TerrainManager.instance.PlayerTutorialStage();
    }


    private void CompanionText(string text, AudioClip sound = null, int pops = 0)
    {
        if (text.Length == 0)
            tutorialCompanionTipBackground.SetActive(false);
        else
        {
            // add sparkles
            tutorialCompanionTipBackground.SetActive(true);
            tutorialCompanionTip.text = text;
            if (pops == 2)
                UIPopManager.instance.ShowPops(tutorialCompanionTipBackground.transform.position, successPopIntensity, successPopColor);
            else if (pops == 1)
                UIPopManager.instance.ShowPops(tutorialCompanionTipBackground.transform.position, minorPopIntensity, minorPopColor);
            if (sound)
                AudioManager.instance.SoundPlayEven(sound, Vector2.zero);
        }
    }

    public void TutorialShopStart(UIDefeatFirst uiDefeatFirst)
    {
        StartCoroutine(TutorialShopSequence(uiDefeatFirst));
    }

    private bool SkippablePause()
    {
        tutorialDelay -= Time.unscaledDeltaTime;
        if (tutorialDelay > 0) return true;
        return false;
    }

    private IEnumerator TutorialShopSequence(UIDefeatFirst uiDefeatFirst)
    {
        companionControls.ShowCompanion();
        CompanionText("");
        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);
        // TODO deactivate the shop button
        CompanionText("Oh no! You're hurt!", progressSound, 1);
        companionControls.CompanionHurt(tutorialShopDelay);
        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);
        tutorialDelay = tutorialShopDelay;
        while (SkippablePause()) yield return new WaitForEndOfFrame();

        CompanionText("It's ok! You can try again!", progressSound, 1);
        companionControls.CompanionHappy(tutorialShopDelay);
        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);
        tutorialDelay = tutorialShopDelay;
        while (SkippablePause()) yield return new WaitForEndOfFrame();

        CompanionText("Here, take these gems!", null, 2);
        companionControls.CompanionTalk(tutorialShopDelay);
        UIPopManager.instance.StartPopTrail(tutorialCompanionTipBackground.transform, UIMenus.instance.menuResources.GetGemsTransform(), tutorialShopDelayMin, Color.red, 100);
        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);

        GameManager.instance.AddGems(tutorialFreeGems);
        GameManager.instance.SetFlag(GlobalVars.SAVEFLAGTUTGEMSGIVEN);
        UIMenus.instance.menuResources.UpdateResources();
        //PlayerPawn.instance.pawnPurse.ChangeBars();

        tutorialDelay = tutorialShopDelay;
        while (SkippablePause()) yield return new WaitForEndOfFrame();

        CompanionText("Now open the upgrade shop!", progressSound, 1);
        companionControls.CompanionTalk(tutorialShopDelay);

        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);
        tutorialShopWait = true;
        uiDefeatFirst.TutorialReady();

        while (tutorialShopWait)
            yield return new WaitForEndOfFrame();
        // reset this immediately - the shop has now been opened so the player could buy an item from this point forward
        tutorialShopWait = true;

        CompanionText("Buy an extra heart!", progressSound, 1);
        companionControls.CompanionTalk(tutorialShopDelay);
        while (tutorialShopWait)
            yield return new WaitForEndOfFrame();

        GameManager.instance.SetFlag(GlobalVars.SAVEFLAGTUTITEMBOUGHT);
        GameManager.instance.SetFlag(GlobalVars.SAVEFLAGTUTORIALSHOP); // setting this here as the crucial gem/item part is saved and should not be repeated
        GameManager.instance.SaveSettings();

        CompanionText("Perfect!", successSound, 2);
        companionControls.CompanionCheer(tutorialResultTime);
        yield return new WaitForSecondsRealtime(tutorialShopDelayMin);
        tutorialDelay = tutorialShopDelay;
        while (SkippablePause()) yield return new WaitForEndOfFrame();

        // shop complete, go to continue screen
        CompanionText("Now try to run further!", successSound, 2);
        uiDefeatFirst.TutorialShopDone();
        // continue button is now active, which will restart the scene, so we're done here!
    }

    public void TutorialShopOpened()
    {
        tutorialShopWait = false;
    }

    // any touch/mouse input will call this
    public void TouchInput()
    {
        tutorialDelay = 0;
    }

    // for now there's just one buy tutorial, so we don't need this to actually know the item right?
    public void ItemBought()
    {
        // only stop the pop rect on the first purchase, not anything the player might be able to afford afterwards
        UIPopManager.instance.StopPopRect();
        tutorialShopWait = false;
    }

    // called by the companion object itself (after being directed to go active by this classs) to establish a single source of control for the companion and their UI components
    // these should not be called from inside this class
    public void ShowCompanion()
    {
        tutorialCompanion.SetActive(true);
    }
    public void HideCompanion()
    {
        tutorialCompanion.SetActive(false);
        CompanionText("");
    }
}
