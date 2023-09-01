using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIPointer
// manages showing the pointer for touch/mouse interactions
// created 24/8/23
// last modified 24/8/23

public class UIPointer : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private Image pointerImage; // the actual pointer object
    [SerializeField] private Color pointerColor;
    [SerializeField] private float pointerFadeDuration = 0.5f; // how long the pointer takes to fade
    private float pointerFadeTime; // time left before pointer fades out

    private void Awake()
    {
        pointerImage.color = Color.clear;
    }
    
    private void Update()
    {
        if (pointerFadeTime > 0)
        {
            // this section is structured this way to ensure that (if updates happen in the wrong order) the pointer doesn't show as part faded constantly
            Color colorFade = pointerColor;
            colorFade.a = pointerFadeTime / pointerFadeDuration;

            // want this to still fade during pause
            pointerFadeTime -= Time.unscaledDeltaTime;

            if (pointerFadeTime <= 0)
                pointerImage.color = Color.clear;
            else
                pointerImage.color = colorFade;
        }
    }

    public void ShowPointer(Vector2 screenPos)
    {
        pointer.position = screenPos;
        pointerImage.color = pointerColor;
        pointerFadeTime = pointerFadeDuration;
    }
}
