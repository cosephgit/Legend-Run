using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIPopMaker
// manages creating pops for a UI element
// created 13/8/24
// modified 13/8/24

public class UIPopMaker : MonoBehaviour
{
    [Header("Pop settings")]
    [SerializeField] private Transform popPos; // if this UI uses pops, they should spawn around this point
    [SerializeField] private Color popColor = Color.yellow;
    [SerializeField] private float popStrength = 1f;

    public void MakePops(float strengthBonus = 0f)
    {
        MakePops(popColor, strengthBonus);
    }
    public void MakePops(Color colorOverride, float strengthBonus = 0f)
    {
        UIPopManager.instance.ShowPops(popPos.position, popStrength + strengthBonus, colorOverride);
    }
}
