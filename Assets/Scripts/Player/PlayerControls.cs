using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// takes player input, shows the pointer and passes the controls on if required
// created 31/8/23
// last modified 1/9/23

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private PlayerPawn player;
    [SerializeField] private UIPointer pointer;
    private Vector3 swipeStart;
    private Vector3 swipeEnd;
    private bool swiping;

    private void Awake()
    {
        swipeStart = Vector3.zero;
        swipeEnd = Vector3.zero;
        swiping = false;
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
        pointer.ShowPointer(touchPos);

        if (touch)
        {
            if (!swiping)
            {
                swiping = true;
                swipeStart = touchPos;
            }
            swipeEnd = touchPos;
            UIPopManager.instance.UpdateTouch(touchPos);
        }
        else
            UIPopManager.instance.EndTouch();

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
    }

    private void SwipeEnd()
    {
        if (player)
        {
            Vector3 swipeVector = swipeEnd - swipeStart;
            swipeStart = Vector3.zero;
            swipeEnd = Vector3.zero;
            swiping = false;
            swipeVector /= Screen.width; // scale swipe to screen resolution, so the x value will be from -0.5 to 0.5 and the y value will be approx. -0.3 to 0.3

            player.SetSwipe(swipeVector);

            UIPopManager.instance.EndTouch();
        }
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
