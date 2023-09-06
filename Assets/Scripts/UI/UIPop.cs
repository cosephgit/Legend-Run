using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIPop
// each individual pop in the UI, handles the individual movements and fade outs
// created 24/8/23
// last modified 6/9/23

public class UIPop : MonoBehaviour
{
    [SerializeField] private Image popImage;
    private UIPopManager parentManager;
    private Color popColor;
    private Vector2 popVee;

    public void Initialise(UIPopManager parentManagerNew)
    {
        parentManager = parentManagerNew;
        gameObject.SetActive(false);
    }

    public void Launch(Vector2 vee, Quaternion rot, Vector2 pos, Color color)
    {
        popColor = color;
        popImage.color = color;
        popVee = vee;
        transform.position = pos;
        transform.rotation = rot;
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
            // apply gravity
            Quaternion rot = transform.rotation * Quaternion.Euler(0f, 0f, popVee.x);
            Vector2 vee = popVee;

            vee.y += parentManager.popGravity * parentManager.popScreenScale * Time.unscaledDeltaTime;

            popVee = vee;
            transform.rotation = rot;
        }
    }
}
