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
    [Header("Rush powerup")]
    public float rushRateBase = 1f;
    public float rushRateBonus = 0.2f;
    [Header("Riches powerup")]
    public float richesRateBase = 1f;
    public float richesRateBonus = 0.2f;
    [Header("Magnet powerup")]
    public float magnetRateBase = 1f;
    public float magnetRateBonus = 0.2f;
    [Header("Sword powerup")]
    public float swordRateBase = 1f;
    public float swordRateBonus = 0.2f;
    [Header("Potion powerup")]
    public float potionRateBase = 1f;
    public float potionRateBonus = 0.2f;
    [Header("Coin rate bonus")]
    public float coinRateBase = 1f;
    public float coinRateBonus = 0.05f;
    [Header("Speed bonus")]
    public float speedBase = 1f;
    public float speedBonus = 0.05f;
    [Header("Health bonus")]
    public int healthBase = 1;
    public int healthBonus = 1;
    [Header("Boost initial bonus")]
    public int boostStartBase = 0;
    public int boostStartBonus = 1;
    [Header("Boost gain rate bonus")]
    public float boostGainBase = 1f;
    public float boostGainBonus = 0.05f;
    [Header("Boost shield bonus")]
    public int boostShieldBase = 0;
    public int boostShieldBonus = 1;
}
