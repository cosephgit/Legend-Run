using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// UIMenuMainScores
// main menu scores display
// created 5/9/23
// last modified 5/9/23

public class UIMenuMainScores : MonoBehaviour
{
    [SerializeField] private GameObject coinsBox;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private GameObject distanceBox;
    [SerializeField] private TextMeshProUGUI distanceText;

    private void Start()
    {
        int coins = GameManager.instance.coinsStash;
        int distance = GameManager.instance.distanceBest;

        if (coins > 0)
        {
            coinsBox.SetActive(true);
            coinsText.text = GameManager.instance.DisplayCoins(coins);
        }
        else
            coinsBox.SetActive(false);

        if (distance > 0)
        {
            distanceBox.SetActive(true);
            distanceText.text = GameManager.instance.DisplayDistance(distance);
        }
        else
            distanceBox.SetActive(false);
    }
}
