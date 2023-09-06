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
    Finished
}


public class PlayerTutorial : MonoBehaviour
{
    [Header("Tutorial UI parameters")]
    [SerializeField] private float tutorialResultTime = 1f;
    [Header("Tutorial UI elements")]
    [SerializeField] private GameObject tutorialBlockTop;
    [SerializeField] private TextMeshProUGUI tutorialBlockTopText;
    [SerializeField] private GameObject tutorialBlockBottom;
    [SerializeField] private TextMeshProUGUI tutorialBlockBottomText;
    [SerializeField] private GameObject tutorialTryAgain; // show when the player gets tutorial step wrong
    [SerializeField] private GameObject tutorialWellDone; // show when the player successfully completes a tutorial step
    [SerializeField] private GameObject tutorialAllDone; // show when the player successfully completes a tutorial step
    [Header("Success effects")]
    [SerializeField] private Color successPopColor = Color.green;
    [SerializeField] private float successPopIntensity = 4f;
    [SerializeField] private AudioClip successSound;
    [Header("Failure effects")]
    [SerializeField] private AudioClip failureSound;
    public TutorialState state { get; private set; } = TutorialState.Init;

    private void Start()
    {
        tutorialBlockTop.SetActive(true);
        tutorialBlockTopText.text = ActionWord() + " Below To Move";
        tutorialBlockBottom.SetActive(false);
        state = TutorialState.Move;
    }

    // select the right action word based on controls connected
    private string ActionWord()
    {
        if (Input.mousePresent)
        {
            if (Input.touchSupported) return "Click/Tap";
            return "Click";
        }

        if (Input.touchSupported) return "Tap";

        Debug.LogError("No valid input detected");
        return "Tap";
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

    // called when a hazard is destroyed without hitting the player
    public void HazardDodged()
    {
        if (state == TutorialState.Dodge)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    // called when the player lands after a hazard is destroyed without hitting the player
    public void HazardJumped()
    {
        if (state == TutorialState.Jump)
        {
            StartCoroutine(TutorialPassed());
        }
    }

    // called when the player hits a hazard
    public void HazardHit()
    {
        if (state == TutorialState.Dodge || state == TutorialState.Jump)
        {
            StartCoroutine(TutorialFailed());
        }
    }

    // called when a tutorial stage is passed
    private IEnumerator TutorialPassed()
    {
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
        }

        tutorialWellDone.SetActive(true);
        UIPopManager.instance.ShowPops(tutorialWellDone.transform.position, successPopIntensity, successPopColor);
        if (successSound) AudioManager.instance.SoundPlayEven(successSound, Vector2.zero);
        yield return new WaitForSeconds(tutorialResultTime);
        tutorialWellDone.SetActive(false);

        switch (state)
        {
            case TutorialState.MoveDone:
                tutorialBlockTopText.text = "Move To Collect Coins";
                state = TutorialState.Collect;
                break;
            case TutorialState.CollectDone:
                tutorialBlockTopText.text = "Move To Dodge Hazards";
                state = TutorialState.Dodge;
                break;
            case TutorialState.DodgeDone:
                tutorialBlockTop.SetActive(false);
                tutorialBlockBottom.SetActive(true);
                tutorialBlockBottomText.text = ActionWord() + " Above To Jump Hazards";
                state = TutorialState.Jump;
                break;
            case TutorialState.JumpDone:
                state = TutorialState.Finished;
                tutorialBlockBottom.SetActive(false);
                StartCoroutine(TutorialComplete());
                break;
        }
    }
    private IEnumerator TutorialFailed()
    {
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
        }

        tutorialTryAgain.SetActive(true);
        if (failureSound) AudioManager.instance.SoundPlayEven(failureSound, Vector2.zero);
        yield return new WaitForSeconds(tutorialResultTime);
        tutorialTryAgain.SetActive(false);

        switch (state)
        {
            case TutorialState.MoveDone:
                state = TutorialState.Move;
                break;
            case TutorialState.CollectDone:
                state = TutorialState.Collect;
                break;
            case TutorialState.DodgeDone:
                state = TutorialState.Dodge;
                break;
            case TutorialState.JumpDone:
                state = TutorialState.Jump;
                break;
        }
    }
    private IEnumerator TutorialComplete()
    {
        tutorialAllDone.SetActive(true);
        UIPopManager.instance.ShowPops(tutorialAllDone.transform.position, successPopIntensity, successPopColor);
        if (successSound) AudioManager.instance.SoundPlayEven(successSound, Vector2.zero);
        yield return new WaitForSeconds(tutorialResultTime);
        tutorialAllDone.SetActive(false);
    }
}
