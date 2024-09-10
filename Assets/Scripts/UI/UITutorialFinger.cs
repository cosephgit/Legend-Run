using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// UITutorialFinger
// manages the finger which shows swiping motions
// created 21/8/24
// modified 21/8/24

public class UITutorialFinger : MonoBehaviour
{
    [SerializeField] private RectTransform fingerRect;
    [SerializeField] private Image finger;
    [SerializeField] private float swipeStartPause = 0.5f;
    [SerializeField] private float swipeTime = 2f;
    [SerializeField] private float swipeEndTime = 1f;
    [SerializeField] private float swipeLoopDelay = 0.5f;


    public void Hide()
    {
        StopAllCoroutines();
        UIPopManager.instance.StopPopRect();
        gameObject.SetActive(false);
    }

    public void ShowSwipe(UITutorialTip tip)
    {
        StopAllCoroutines();
        UIPopManager.instance.StopPopRect();
        finger.color = Color.white;
        gameObject.SetActive(true);
        StartCoroutine(SwipeSequence(tip.swipeTipStart, tip.swipeTipEnd, tip.swipeOscillate));
    }

    private IEnumerator SwipeSequence(Transform start, Transform end, bool oscillate)
    {
        Color colorFade = Color.white;
        Vector3 pos;
        Vector3 offset = end.position - start.position;
        float progress;
        bool reverse = false;

        while (true)
        {
            // pause at start point
            colorFade = Color.white;
            finger.color = colorFade;
            if (reverse)
                transform.position = end.position;
            else
                transform.position = start.position;
            UIPopManager.instance.StartPopRect(fingerRect, Color.white, true);

            yield return new WaitForSeconds(swipeStartPause);

            // swipe to end point
            progress = 0f;
            while (progress < swipeTime)
            {
                if (reverse)
                    pos = end.position - (progress / swipeTime) * offset;
                else
                    pos = start.position + (progress / swipeTime) * offset;
                transform.position = pos;
                progress += Time.deltaTime;
                yield return new WaitForNextFrameUnit();
            }
            if (reverse)
                transform.position = start.position;
            else
                transform.position = end.position;

            UIPopManager.instance.StopPopRect();

            // pull away/fade out and disappear
            progress = swipeEndTime;
            while (progress > 0)
            {
                progress -= Time.deltaTime;
                colorFade.a = progress / swipeEndTime;
                finger.color = colorFade;
                yield return new WaitForNextFrameUnit();
            }
            colorFade.a = 0f;
            finger.color = colorFade;

            // loop
            yield return new WaitForSeconds(swipeLoopDelay);
            if (oscillate) reverse = !reverse;
        }
    }
}
