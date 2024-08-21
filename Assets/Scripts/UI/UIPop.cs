using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIPop
// each individual pop in the UI, handles the individual movements and fade outs
// created 6/9/23
// last modified 6/9/23

public enum PopType
{
    Pop,
    Sparkle
}

public class UIPop : MonoBehaviour
{
    [SerializeField] private Image popImage;
    [field: SerializeField] public PopType popType { get; private set; } = PopType.Pop;
    private UIPopManager parentManager;
    private Color popColor;
    private Vector2 popVee;
    private float fadeTimeLeft;
    private float fadeTimeFull;

    public void Initialise(UIPopManager parentManagerNew)
    {
        parentManager = parentManagerNew;
        gameObject.SetActive(false);
    }

    public void Launch(Vector2 vee, Quaternion rot, Vector2 pos, Color color, float fadeTime)
    {
        popColor = color;
        popImage.color = color;
        popVee = vee;
        transform.position = pos;
        transform.rotation = rot;
        if (fadeTime > 0) fadeTimeLeft = fadeTime;
        else fadeTimeLeft = 0f; // in case a previous usage left a value behind
        fadeTimeFull = fadeTimeLeft;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Vector2 pos = transform.position;

        pos += popVee * Time.unscaledDeltaTime;

        transform.position = pos;

        if (pos.y < -100f)
        {
            // deactivate when off screen
            parentManager.RestorePop(this);
            gameObject.SetActive(false);
        }
        else
        {
            if (fadeTimeFull > 0)
            {
                fadeTimeLeft -= Time.deltaTime;
                if (fadeTimeLeft <= 0)
                {
                    // fade out finished, deactivate
                    parentManager.RestorePop(this);
                    gameObject.SetActive(false);
                    return;
                }
                Color colorFaded = popColor;
                popColor.a *= fadeTimeLeft / fadeTimeFull;
                popImage.color = popColor;
            }

            // apply gravity
            Vector2 vee = popVee;

            if (popType == PopType.Pop)
                vee.y += parentManager.popGravity * parentManager.popScreenScale * Time.unscaledDeltaTime;
            else
                vee.y += parentManager.sparkleGravity * parentManager.popScreenScale * Time.unscaledDeltaTime;

            popVee = vee;

            Quaternion rot = transform.rotation * Quaternion.Euler(0f, 0f, popVee.x);
            transform.rotation = rot;
        }
    }
}
