using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIHealthHearts
// manages the health display
// created 12/8/24
// modified 12/8/24

public class UIHealthHearts : MonoBehaviour
{
    [SerializeField] private RectTransform heartGrid; // needed to force update at times
    [SerializeField] private UIHealthHeart heartBase; // the first health heart and template for all further hearts
    private List<UIHealthHeart> hearts;
    private int healthCurrent = 1;
    private int healthCurrentMax = 1;

    public void Initialise(int healthMax)
    {
        hearts = new List<UIHealthHeart>() { heartBase };
        for (int i = healthCurrentMax; i < healthMax; i++)
        {
            UIHealthHeart heartNew = Instantiate(heartBase, heartBase.transform.parent);
            hearts.Add(heartNew);
        }
        for (int i = 0; i < healthMax; i++)
        {
            hearts[i].Initialise();
            hearts[i].Fill(false);
        }
        healthCurrent = healthMax;
        healthCurrentMax = healthMax;
    }

    public void ChangeHealthMax(int healthMaxNew)
    {
        if (healthMaxNew > healthCurrentMax)
        {
            for (int i = healthCurrentMax; i < healthMaxNew; i++)
            {
                UIHealthHeart heartNew = Instantiate(heartBase, heartBase.transform.parent);
                hearts.Add(heartNew);
                heartNew.Initialise();
            }
            healthCurrentMax = healthMaxNew;
        }
    }

    public void ChangeHealth(int health)
    {
        if (health > healthCurrentMax)
            ChangeHealthMax(health);
        for (int i = 0; i < hearts.Count; i++)
        {
            if (health < healthCurrent)
            {
                // health is reducing, so empty excess hearts
                if (i >= health)
                {
                    hearts[i].Empty();
                }
            }
            else
            {
                // health is increasing, so fill renewed hearts
                if (i < health)
                {
                    hearts[i].Fill();
                }
            }
        }
        healthCurrent = health;
    }
}
