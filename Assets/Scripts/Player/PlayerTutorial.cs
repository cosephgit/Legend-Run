using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// PlayerTutorial
// manages the tutorial screens the first time the player plays the game
// created 6/9/23
// last modified 6/9/23


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
    [SerializeField] private UITutorialTip tipMove;
    [SerializeField] private TextMeshProUGUI tutorialBlockTopText;

    [SerializeField] private UITutorialTip tipJump;
    [SerializeField] private TextMeshProUGUI tutorialBlockBottomText;

    [SerializeField] private GameObject tutorialTryAgain; // show when the player gets tutorial step wrong
    [SerializeField] private GameObject tutorialWellDone; // show when the player successfully completes a tutorial step
    [SerializeField] private GameObject tutorialAllDone; // show when the player successfully completes a tutorial step
    [SerializeField] private UITutorialFinger tutorialFinger;
    [Header("Success effects")]
    [SerializeField] private Color successPopColor = Color.green;
    [SerializeField] private float successPopIntensity = 4f;
    [SerializeField] private AudioClip successSound;
    [Header("Failure effects")]
    [SerializeField] private AudioClip failureSound;
    public TutorialState state { get; private set; } = TutorialState.Init;

    private void Start()
    {
        tutorialFinger.Hide();
        if (GameManager.instance.tutorialDone)
        {
            tipMove.gameObject.SetActive(false);
            tipJump.gameObject.SetActive(false);
            state = TutorialState.Finished;
            TerrainManager.instance.PlayerTutorialStage();
        }
        else
        {
            tipMove.gameObject.SetActive(true);
            tipJump.gameObject.SetActive(false);
            state = TutorialState.Move;
            PlayerPawn.instance.TutorialStart();
            tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
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

        tutorialWellDone.SetActive(true);
        UIPopManager.instance.ShowPops(tutorialWellDone.transform.position, successPopIntensity, successPopColor);
        if (successSound) AudioManager.instance.SoundPlayEven(successSound, Vector2.zero);
        yield return new WaitForSeconds(tutorialResultTime);
        tutorialWellDone.SetActive(false);

        switch (state)
        {
            case TutorialState.MoveDone:
                tutorialBlockTopText.text = "Collect Coins";
                tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
                state = TutorialState.Collect;
                break;
            case TutorialState.CollectDone:
                tutorialBlockTopText.text = "Dodge Hazards";
                tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
                state = TutorialState.Dodge;
                break;
            case TutorialState.DodgeDone:
                tipMove.gameObject.SetActive(false);
                tipJump.gameObject.SetActive(true);
                tutorialFinger.ShowSwipe(tipJump.swipeTipStart, tipJump.swipeTipEnd, tipJump.swipeOscillate);
                state = TutorialState.Jump;
                break;
            case TutorialState.JumpDone:
                tutorialBlockBottomText.text = "Jump hazards";
                tutorialFinger.ShowSwipe(tipJump.swipeTipStart, tipJump.swipeTipEnd, tipJump.swipeOscillate);
                state = TutorialState.JumpOver;
                break;
            case TutorialState.JumpOverDone:
                tipJump.gameObject.SetActive(false);
                yield return new WaitForSeconds(tutorialResultTime);
                tutorialAllDone.SetActive(true);
                UIPopManager.instance.ShowPops(tutorialAllDone.transform.position, successPopIntensity, successPopColor);
                if (successSound) AudioManager.instance.SoundPlayEven(successSound, Vector2.zero);
                yield return new WaitForSeconds(tutorialResultTime);
                tutorialAllDone.SetActive(false);
                GameManager.instance.TutorialComplete();
                state = TutorialState.Finished;
                break;
        }
        TerrainManager.instance.PlayerTutorialStage();
    }

    private IEnumerator TutorialFailed()
    {
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

        tutorialTryAgain.SetActive(true);
        if (failureSound) AudioManager.instance.SoundPlayEven(failureSound, Vector2.zero);
        yield return new WaitForSeconds(tutorialResultTime);
        tutorialTryAgain.SetActive(false);

        switch (state)
        {
            case TutorialState.MoveDone:
                state = TutorialState.Move;
                tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
                break;
            case TutorialState.CollectDone:
                state = TutorialState.Collect;
                tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
                break;
            case TutorialState.DodgeDone:
                state = TutorialState.Dodge;
                tutorialFinger.ShowSwipe(tipMove.swipeTipStart, tipMove.swipeTipEnd, tipMove.swipeOscillate);
                break;
            case TutorialState.JumpDone:
                state = TutorialState.Jump;
                tutorialFinger.ShowSwipe(tipJump.swipeTipStart, tipJump.swipeTipEnd, tipJump.swipeOscillate);
                break;
            case TutorialState.JumpOverDone:
                state = TutorialState.JumpOver;
                tutorialFinger.ShowSwipe(tipJump.swipeTipStart, tipJump.swipeTipEnd, tipJump.swipeOscillate);
                break;
        }

        TerrainManager.instance.PlayerTutorialStage();
    }



    /*
     * so here we need some tutorial steps (maybe call the player tutorial class to start it?)
     * "oh no you're hurt!"
     * "don't worry!"
     * "just pick yourself up and try again!"
     * "here are some gems, they're rare and powerful!"
     * "use the gems to buy this health heart!"
     * "run again, now with two hearts!"
     */
    public void TutorialShopStart()
    {
        StartCoroutine(TutorialShopSequence());
    }

    private IEnumerator TutorialShopSequence()
    {
        tipMove.gameObject.SetActive(true);
        tutorialBlockTopText.text = "Dodge Hazards";
        yield return new WaitForEndOfFrame();
    }
}
