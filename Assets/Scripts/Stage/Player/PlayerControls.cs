using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// takes player input, shows the pointer and passes the controls on if required
// created 31/8/23
// last modified 1/9/23

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private PlayerPawn player;
    [SerializeField] private float swipeSensitivitySide = 0.5f;
    [SerializeField] private float swipeSensitivityUp = 1f;
    [SerializeField] private float swipeHoldCooldown = 0.5f;
    [SerializeField] private float swipeSensitivityMin = 0.1f; // min per second to count as swiping
    [SerializeField] private UIPointer pointer;
    private Vector2 swipeStart;
    private Vector2 swipeLast;
    private Vector2 swipeEnd;
    private bool swiping;
    private bool swipePrep;
    private float swipeCooling;

    private void Awake()
    {
        swipeStart = Vector3.zero;
        swipeEnd = Vector3.zero;
        swiping = false;
        swipePrep = false;
        swipeCooling = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private List<RaycastResult> GetUIObjects(Vector2 pos)
    {
        // check if the object is the interactionmenu
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    private void ShowPointer(Vector2 touchPos, bool touch)
    {
#if UNITY_EDITOR || !UNITY_ANDROID
        pointer.ShowPointer(touchPos);
#endif

        if (touch)
        {
            bool moving = ((touchPos - swipeLast).magnitude > swipeSensitivityMin);

            swipeLast = touchPos;
            swipeEnd = touchPos;
            UIPopManager.instance.UpdateTouch(touchPos);

            if (!swiping)
            {
                swiping = true;
                swipePrep = true;
                swipeCooling = swipeHoldCooldown;
                swipeStart = touchPos;
            }
            else if (swipePrep) // check each frame if we should send the swipe to the player
                SwipeNew();

            if (!moving || !swipePrep)
            {
                swipeCooling -= Time.deltaTime;
                if (swipeCooling < 0)
                {
                    swiping = false;
                    swipeStart = Vector3.zero;
                    swipeEnd = Vector3.zero;
                    UIPopManager.instance.EndTouch();
                }
            }
        }
        else
            UIPopManager.instance.EndTouch();

        /*
        if (player && touch)
        {
            List<RaycastResult> objectsUI = GetUIObjects(touchPos);

            foreach (RaycastResult objectUI in objectsUI)
            {
                Button objectButton = objectUI.gameObject.GetComponentInChildren<Button>();
                if (objectButton) return;
            }

            player.SetMove(touchPos);
        }
        */
    }

    private void SwipeNew()
    {
        if (player)
        {
            Vector3 swipeVector = swipeEnd - swipeStart;

            if (Mathf.Abs(swipeVector.x) > swipeVector.y)
                swipeVector.y = 0f;
            else
                swipeVector.x = 0f;

            if (swipeVector.y > swipeSensitivityUp)
            {
                swipeStart = Vector3.zero;
                swipeEnd = Vector3.zero;
                player.SetSwipe(Vector3.up);
                swipePrep = false;
            }
            else if (swipeVector.x > swipeSensitivitySide)
            {
                swipeStart = Vector3.zero;
                swipeEnd = Vector3.zero;
                player.SetSwipe(Vector3.right);
                swipePrep = false;
            }
            else if (swipeVector.x < -swipeSensitivitySide)
            {
                swipeStart = Vector3.zero;
                swipeEnd = Vector3.zero;
                player.SetSwipe(Vector3.left);
                swipePrep = false;
            }
            /*
            if (swipeVector.x > swipeSensitivitySide
                || swipeVector.y > swipeSensitivityUp)
            {
                swipeStart = Vector3.zero;
                swipeEnd = Vector3.zero;
                swipeVector /= Screen.width; // scale swipe to screen resolution, so the x value will be from -0.5 to 0.5 and the y value will be approx. -1 to 1

                player.SetSwipe(swipeVector);

                swipePrep = false;
                // trying this for now - so it will immediately pick up a new swipe on the next frame
                swiping = false;
                UIPopManager.instance.EndTouch();
            }
            */
        }
    }

    private void SwipeEnd()
    {
        if (player)
        {
            Vector3 swipeVector = swipeEnd - swipeStart;
            swipeStart = Vector3.zero;
            swipeEnd = Vector3.zero;
            swipeVector /= Screen.width; // scale swipe to screen resolution, so the x value will be from -0.5 to 0.5 and the y value will be approx. -0.3 to 0.3

            //player.SetSwipe(swipeVector);
            player.SwipeEnd(swipeVector);

        }
        swiping = false;
        UIPopManager.instance.EndTouch();
    }

    private void Update()
    {
        bool touch = false;

        if (Input.touchCount > 0)
        {
            touch = true;
            ShowPointer(Input.touches[0].position, touch);
        }
        else if (Input.mousePresent)
        {
            touch = Input.GetMouseButton(0);
            ShowPointer(Input.mousePosition, touch);
        }
        if (swiping && !touch)
            SwipeEnd();
    }
}
