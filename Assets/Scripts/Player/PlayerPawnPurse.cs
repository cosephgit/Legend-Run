using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player coin manager component
// created 23/8/23
// last modified 6/9/23

public class PlayerPawnPurse : MonoBehaviour
{
    [Header("Coin settings")]
    [SerializeField] private UIResourceBar coinBar;
    [SerializeField] private UIResourceBar gemBar;
    public int coins { get; private set; }
    public int gems { get; private set; }

    private void Start()
    {
        coins = GameManager.instance.coinsStash;
        gems = GameManager.instance.gemsStash;
    }

    public void ChangeBars()
    {
        if (coins != GameManager.instance.coinsStash)
        {
            coins = GameManager.instance.coinsStash;
            coinBar.SetValue(coins);
        }
        if (gems != GameManager.instance.gemsStash)
        {
            gems = GameManager.instance.gemsStash;
            gemBar.SetValue(gems);
        }
    }
}
