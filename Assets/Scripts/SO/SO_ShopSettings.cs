using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SO_ShopSettings
// contains the settings for the shop and shop items
// list of all shop items
// settings for shop item effectiveness (e.g. how much bonus to give)

public enum ItemType
{
    RushUnlock, // unlock the rush powerup
    RushRate, // float: base rate *= (1+(rate bonuses))
    RichesUnlock, // unlock the riches powerup
    RichesRate, // float: base rate *= (1+(rate bonuses))
    MagnetUnlock, // unlock the magnet powerup
    MagnetRate, // float: base rate *= (1+(rate bonuses))
    SwordUnlock, // unlock the sword powerup
    SwordRate, // float: base rate *= (1+(rate bonuses))
    PotionUnlock, // unlock the potion powerup
    PotionRate, // float: base rate *= (1+(rate bonuses))
    CoinRate, // float: base rate *= (1+(rate bonuses))
    SpeedUp,
    HealthUp,
    BoostInitial,
    BoostGain,
    BoostShield,
    BuyGems
}

[CreateAssetMenu(fileName = "ShopSettings", menuName = "ScriptableObjects/ShopSettings", order = 1)]
public class SO_ShopSettings : ScriptableObject
{
    public SO_ShopItem[] shopItems;
    public SO_ShopItem[] shopItemsGems;
    public Sprite spriteCostCoin;
    public Sprite spriteCostGem;
    public Sprite spriteCostPremium;
    public Sprite spriteStateLocked;
    public Sprite spriteStateOwned;
    [Header("Extra life cost")]
    public int reviveGemCost = 12;

    public SO_ShopItem GetShopItem(string itemName)
    {
        SO_ShopItem result = null;

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i].shopUniqueName == itemName)
                return shopItems[i];
        }

        return result;
    }
}
