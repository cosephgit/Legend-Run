using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player coin manager component
// created 23/8/23
// last modified 6/9/23

public class PlayerPawnPurse : MonoBehaviour
{
    [Header("Coin settings")]
    [SerializeField] private UICoinBar coinBar;
    public int coins { get; private set; }

    private void Start()
    {
        coins = 0;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinBar.SetValue(coins);
    }
}
