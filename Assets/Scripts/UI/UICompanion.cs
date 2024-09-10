using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// UICompanion
// manages the tutorial companion, animation, enabling, etc
// created 23/8/24
// modified 23/8/24

public class UICompanion : MonoBehaviour
{
    private const string ANIMTRIGHURT = "Hurt";
    private const string ANIMBOOLRUN = "Run";
    [SerializeField] private Transform companionOrigin; // define the companion base/home point
    [SerializeField] private GameObject companion;
    [SerializeField] private Transform companionPivot; // the object to pivot for rotating the companion
    [SerializeField] private Animator companionAnim;
    [SerializeField] private Camera companionCam; // renders to render texture to show in the UI
    [SerializeField] private float maxTurnSpeed = 45f;
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float baseMoveDuration = 2f;
    [SerializeField] private float posBelowY = -2f;
    [SerializeField] private float posAboveY = 2f;
    [Header("Default position")]
    [FormerlySerializedAs("pitchNormal")][SerializeField] private float defaultPitch = 15f;
    [Header("Talk sequence")]
    [SerializeField] private float talkNodRate = 4f;
    [SerializeField] private float talkNodMagnitude = 10f;
    [SerializeField] private float talkShakeRate = 2f;
    [SerializeField] private float talkShakeMagnitude = 5f;
    [SerializeField] private bool talkSnapTo = true;
    [Header("Happy sequence")]
    [SerializeField] private float happyPitchDelta = -20f;
    [SerializeField] private float happyYDelta = 0.5f;
    [SerializeField] private bool happySnapTo = true;
    [Header("Cheer sequence")]
    [SerializeField] private AnimationCurve cheerYCurve;
    [SerializeField] private AudioClip cheerSound;
    [FormerlySerializedAs("baseSpiralRadius")][SerializeField] private float cheerSpiralRadius = 0.5f; // how big a radius the companion should spiral on
    [FormerlySerializedAs("spiralCountCheer")][SerializeField] private float cheerSpiralCount = 4;
    [Header("Sad sequence")]
    [FormerlySerializedAs("pitchSad")][SerializeField] private float sadPitch = 25f;
    [SerializeField] private float sadShakeMagnitude = 15f;
    [SerializeField] private bool sadSnapTo = true;
    [Header("Mad sequence")]
    [FormerlySerializedAs("pitchMad")][SerializeField] private float madPitch = 5f;
    [Header("Leave sequence")]
    [SerializeField] private AnimationCurve leaveYCurve;
    [SerializeField] private float leaveSpiralRadius = 0.25f;
    [SerializeField] private float leaveSpiralCount = 4;
    private Coroutine coroutineCurrent = null;
    private Coroutine coroutineCurrentSpiral = null;


    // make sure the game object and the UI image are on
    public void SetActive()
    {
        gameObject.SetActive(true);
        if (PlayerPawn.instance)
            PlayerPawn.instance.tutorial.ShowCompanion();
    }
    // make sure the game object and the UI image are off
    public void SetInactive()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
        coroutineCurrent = null;
        coroutineCurrentSpiral = null;
        if (PlayerPawn.instance)
            PlayerPawn.instance.tutorial.HideCompanion();
    }

    private void SetStartPosition()
    {
        Vector3 pos = companionOrigin.position;
        pos.y += posBelowY;
        companion.transform.position = pos;
        companionPivot.rotation = Quaternion.identity;
    }

    public void ShowCompanion()
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionShowSequence());
    }

    private IEnumerator CompanionShowSequence()
    {
        SetStartPosition();

        yield return new WaitForEndOfFrame();

        StartCoroutine(FaceCamera());
    }

    public void CompanionHurt(float duration)
    {
        SetActive();
        SetStartPosition();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionHurtSequence(duration));
    }

    public void CompanionSad(float duration)
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionSadSequence(duration));
    }

    public void CompanionTalk(float duration)
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionTalkSequence(duration));
    }

    public void CompanionHappy(float duration)
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionHappySequence(duration));
    }

    public void CompanionCheer(float duration)
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionCheerSequence(duration));
    }

    private IEnumerator CompanionHurtSequence(float duration)
    {
        companionAnim.SetTrigger(ANIMTRIGHURT);

        yield return new WaitForEndOfFrame();

        coroutineCurrent = StartCoroutine(FaceCamera());
    }

    private IEnumerator CompanionSadSequence(float duration)
    {
        float timeSpent = 0f;
        Vector3 pointSad = companionOrigin.position;
        pointSad.y -= 0.5f;

        while (timeSpent < duration)
        {
            float progress = timeSpent / duration;
            float yaw = Mathf.Sin(progress * 2f * Mathf.PI) * sadShakeMagnitude;

            TurnTowardsPointFrame(companionCam.transform.position, sadPitch, yaw, sadSnapTo);

            MoveTowardsPointFrame(pointSad);

            yield return new WaitForEndOfFrame();
            timeSpent += Time.unscaledDeltaTime;
        }

        coroutineCurrent = StartCoroutine(FaceCamera());
    }

    private IEnumerator CompanionTalkSequence(float duration)
    {
        float timeTaken = 0f;

        while (timeTaken < duration)
        {
            float pitch = defaultPitch + Mathf.Sin(timeTaken * 2f * Mathf.PI * talkNodRate) * talkNodMagnitude;
            float yaw = Mathf.Sin(timeTaken * 2f * Mathf.PI * talkShakeRate) * talkShakeMagnitude;

            TurnTowardsPointFrame(companionCam.transform.position, pitch, yaw, talkSnapTo);

            timeTaken += Time.unscaledDeltaTime;

            yield return new WaitForEndOfFrame();
        }

        coroutineCurrent = StartCoroutine(FaceCamera());
    }

    private IEnumerator CompanionHappySequence(float duration)
    {
        float durationHalf = duration * 0.5f; // nod for half then talk for half
        float timeTaken = 0f;

        while (timeTaken < durationHalf)
        {
            float progress = timeTaken / durationHalf;
            float progressCurve = Mathf.Sin(progress * 2f * Mathf.PI);
            float pitch = defaultPitch + progressCurve * happyPitchDelta;

            TurnTowardsPointFrame(companionCam.transform.position, pitch, 0f, happySnapTo);

            timeTaken += Time.unscaledDeltaTime;

            Vector3 pos = companionOrigin.position;
            pos.y += progressCurve * happyYDelta;

            MoveTowardsPointFrame(pos);

            yield return new WaitForEndOfFrame();
        }

        coroutineCurrent = StartCoroutine(FaceCamera());
        //coroutineCurrent = StartCoroutine(CompanionTalkSequence(durationHalf));
    }

    private IEnumerator CompanionCheerSequence(float duration)
    {
        /*
         * go into flying animation
         * go in fast tight spiral upwards
         * float down to normal position in normal animation and face the camera
         */
        AudioManager.instance.SoundPlayVaried(cheerSound, Vector2.zero);
        StartSpiralMove(companionOrigin.position, cheerYCurve, duration, cheerSpiralRadius, cheerSpiralCount);
        while (coroutineCurrentSpiral != null)
            yield return new WaitForEndOfFrame();

        coroutineCurrent = StartCoroutine(FaceCamera());
    }

    public void HideCompanion(float duration)
    {
        SetActive();

        if (coroutineCurrent != null)
            StopCoroutine(coroutineCurrent);
        if (coroutineCurrentSpiral != null)
        {
            StopCoroutine(coroutineCurrentSpiral);
            coroutineCurrentSpiral = null;
        }

        coroutineCurrent = StartCoroutine(CompanionHideSequence(duration));
    }

    private IEnumerator CompanionHideSequence(float duration)
    {
        StartSpiralMove(companionOrigin.position, leaveYCurve, duration, leaveSpiralRadius, leaveSpiralCount);

        while (coroutineCurrentSpiral != null)
            yield return new WaitForEndOfFrame();

        coroutineCurrent = null;
        companionAnim.SetBool(ANIMBOOLRUN, false);
        companionPivot.rotation = Quaternion.identity;
        companion.transform.position = companionOrigin.position;

        SetInactive();
    }

    // turn the companion towards the target point for one Update frame
    private void TurnTowardsPointFrame(Vector3 point, float pitchAdjust, float yawAdjust, bool snapTo = false)
    {
        // adjust angles - we adjust rotation of the companionAnim component only!
        Vector3 offset = point - companion.transform.position;
        Vector3 targetEuler = new Vector3();
        // calculate yaw
        targetEuler.y = Mathf.Atan2(offset.x, offset.z) * 180 / Mathf.PI + yawAdjust;
        // calculate pitch
        float offsetFlat = Mathf.Sqrt((offset.x * offset.x) + (offset.z * offset.z));
        targetEuler.x = Mathf.Atan2(-offset.y, offsetFlat) * 180 / Mathf.PI + pitchAdjust;

        if (snapTo)
        {
            companionPivot.eulerAngles = targetEuler;
        }
        else
        {
            // get the difference from current angles to target angles
            Vector3 offsetEuler = CoSephUtils.ClampAngleAll(targetEuler - companionPivot.eulerAngles);
            // adjust for turn rate per frame
            float offsetEulerTotal = Mathf.Abs(offsetEuler.x) + Mathf.Abs(offsetEuler.y);
            float frameMaxTurn = maxTurnSpeed * Time.unscaledDeltaTime;

            if (offsetEulerTotal > frameMaxTurn)
            {
                offsetEuler *= frameMaxTurn / offsetEulerTotal;
                companionPivot.eulerAngles += offsetEuler;
            }
            else
                companionPivot.eulerAngles = targetEuler;
        }
    }
    private void MoveTowardsPointFrame(Vector3 point)
    {
        // adjust position - we adjust position of the companion component only!
        Vector3 offset = companionOrigin.transform.position - companion.transform.position;
        float distance = offset.magnitude;
        float frameMaxMove = maxMoveSpeed * Time.unscaledDeltaTime;
        if (distance > frameMaxMove)
        {
            offset *= frameMaxMove / distance;
            companion.transform.position += offset;
        }
        else
            companion.transform.position = companionOrigin.transform.position;
    }

    // make the companion face the camera smoothly after a complete animation
    // Y is yaw, X is pitch
    private IEnumerator FaceCamera()
    {
        while (true)
        {
            TurnTowardsPointFrame(companionCam.transform.position, defaultPitch, 0f);

            MoveTowardsPointFrame(companionOrigin.position);

            yield return new WaitForEndOfFrame();
        }
    }

    private void StartSpiralMove(Vector3 origin, AnimationCurve curve, float moveDuration, float spiralRadius = 0f, float spiralSpinCount = 0f)
    {
        if (coroutineCurrentSpiral != null)
            StopCoroutine(coroutineCurrentSpiral);

        coroutineCurrentSpiral = StartCoroutine(SpiralMove(origin, curve, moveDuration, spiralRadius, spiralSpinCount));
    }

    private IEnumerator SpiralMove(Vector3 origin, AnimationCurve curve, float moveDuration, float spiralRadius, float spiralSpinCount)
    {
        float timeSpent = 0;
        bool spiral = !(spiralRadius == 0 || spiralSpinCount == 0);
        Vector3 pos;
        Vector3 delta;
        // TODO? actually calculate two perpendicular vectors to the vector (end - start)?
        Vector3 spiralX = Vector3.forward;
        Vector3 spiralY = Vector3.right;
        float angleBase = companionPivot.eulerAngles.y;

        companionAnim.SetBool(ANIMBOOLRUN, true);

        while (timeSpent < moveDuration)
        {
            float progress = timeSpent / moveDuration; // CoSephUtils.CalcSCurve(timeSpent / moveDuration);
            pos = origin;
            pos.y += curve.Evaluate(progress);
            //Debug.Log("SpiralMove curve.Evaluate(progress) " + curve.Evaluate(progress) + " progress " + progress);
            //pos = Vector3.Lerp(start, end, progress);
            if (spiral)
            {
                float angle = 2f * Mathf.PI * progress * spiralSpinCount + angleBase;
                float offset = Mathf.Sin(progress * Mathf.PI) * spiralRadius;
                pos += spiralX * Mathf.Sin(angle) * offset;
                pos += spiralY * Mathf.Cos(angle) * offset;
            }
            delta = pos - companion.transform.position;
            companion.transform.position = pos;
            companionPivot.rotation = Quaternion.LookRotation(delta);
            timeSpent += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        companionAnim.SetBool(ANIMBOOLRUN, false);
        coroutineCurrentSpiral = null;
    }
}
