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

    public void Initialise()
    {
        actualHeart.Initialise();
    }

    public void Fill(bool noisy = true)
    {
        heart.sprite = heartFull;
        if (noisy)
        {
            popMaker.MakePops();
            actualHeart.AddShake(1f);
        }
    }
    public void Empty()
    {
        heart.sprite = heartEmpty;
        actualHeart.AddShake(2f);
    }
    public void New()
    {
        heart.sprite = heartNew;
        popMaker.MakePops(Color.magenta);
        actualHeart.AddShake(2f);
    }
}
