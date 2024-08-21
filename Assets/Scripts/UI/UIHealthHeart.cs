using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIHealthHeart
// the individual health heart for the display
// created 12/8/24
// modified 12/8/24

public class UIHealthHeart : MonoBehaviour
{
    [SerializeField] private Image heart;
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] private Sprite heartNew;
    [SerializeField] private UIBase actualHeart; // the actual heart, used for the pops
    [SerializeField] private UIPopMaker popMaker;
    private bool ready;

    public void Initialise()
    {
        StartCoroutine(DelayedInitialise());
    }

    public void Fill(bool noisy = true)
    {
        heart.sprite = heartFull;
        if (noisy)
        {
            popMaker.MakePops();
            if (ready)
                actualHeart.AddShake(1f);
        }
    }
    public void Empty()
    {
        heart.sprite = heartEmpty;
        if (ready)
            actualHeart.AddShake(2f);
    }
    public void New()
    {
        heart.sprite = heartNew;
        if (ready)
            actualHeart.AddShake(2f);
    }

    // need to wait a frame to ensure UI has updated post-instantiation
    private IEnumerator DelayedInitialise()
    {
        yield return new WaitForEndOfFrame();
        actualHeart.Initialise();
        ready = true;
    }
}
