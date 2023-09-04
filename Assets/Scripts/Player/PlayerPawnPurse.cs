using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player coin manager component
// created 23/8/23
// last modified 23/8/23

public class PlayerPawnPurse : MonoBehaviour
{
    [Header("Coin settings")]
    [SerializeField] private UICoinBar coinBar;
    public int coins { get; private set; }

    private void Awake()
    {
        coins = 0;
        coinBar.SetValue(coins, true);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinBar.SetValue(coins);
    }
}
