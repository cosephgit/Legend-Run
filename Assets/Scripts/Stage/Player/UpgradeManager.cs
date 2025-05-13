using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UpgradeManager
// stores values for the upgrade system, single provider for all game levels, attached to GameManager
// created 16/8/24
// modifed 16/8/24

public class UpgradeManager : MonoBehaviour
{
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
    public float speedBonus = 0.02f;
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
    // upgrade level tallies
    private int upgradeRush;
    private int upgradeRiches;
    private int upgradeMagnet;
    private int upgradeSword;
    private int upgradePotion;
    private int upgradeCoin;
    private int upgradeSpeed;
    private int upgradeHealth;
    private int upgradeBoostI;
    private int upgradeBoostG;
    private int upgradeBoostS;
    // actual calculated upgrade values
    // powerup-type upgrades - the "rate" is combined with the default item spawn rate
    public float upgradeRushRate { get; private set; } // TO IMPLEMENT
    public float upgradeRichesRate { get; private set; }  // TO IMPLEMENT
    public float upgradeMagnetRate { get; private set; } // TO IMPLEMENT
    public float upgradeSwordRate { get; private set; } // IMPLEMENTED
    public float upgradePotionRate { get; private set; } // IMPLEMENTED
    public float upgradeCoinRate { get; private set; } // functionally similar to the above powerup spawns, just note the math is different for coins vs powerups - IMPLEMENTED
    public float upgradeSpeedScale { get; private set; } // multiplies the player's move speed - IMPLEMENTED
    public int upgradeHealthPoints { get; private set; } // sets the player's max health - IMPLEMENTED
    public int upgradeBoostInitial { get; private set; } // sets the base/minimum boost level - IMPLEMENTED
    public float upgradeBoostGainRate { get; private set; } // scales the rate at which boost increases - IMPLEMENTED
    public int upgradeBoostShield { get; private set; } // sets the max number of boost shields - TO IMPLEMENT

    public void Initialise()
    {
        // at the start of each game scene, calculate all currently purchased upgrades and set the values
        upgradeRush = 0;
        upgradeRiches = 0;
        upgradeMagnet = 0;
        upgradeSword = 0;
        upgradePotion = 0;
        upgradeCoin = 0;
        upgradeSpeed = 0;
        upgradeHealth = 0;
        upgradeBoostI = 0;
        upgradeBoostG = 0;
        upgradeBoostS = 0;
        List<SO_ShopItem> itemsOwned = GameManager.instance.GetUpgradesOwned();
        for (int i = 0; i < itemsOwned.Count; i++)
        {
            ApplyItemUpgrade(itemsOwned[i]);
        }
        Debug.Log("itemsOwned contains " + itemsOwned.Count + " entries upgradeRush " + upgradeRush + " upgradeRiches " + upgradeRiches + " upgradeMagnet " + upgradeMagnet + " upgradeSword " + upgradeSword + " upgradePotion " + upgradePotion + " upgradeCoin " + upgradeCoin + " upgradeSpeed " + upgradeSpeed + " upgradeHealth " + upgradeHealth + " upgradeBoostI " + upgradeBoostI + " upgradeBoostG " + upgradeBoostG + " upgradeBoostS " + upgradeBoostS);
        CalculateUpgradeEffectAll();
    }

    private void ApplyItemUpgrade(SO_ShopItem itemNew)
    {
        switch (itemNew.itemType)
        {
            case ItemType.RushUnlock:
            case ItemType.RushRate:
                upgradeRush++;
                break;
            case ItemType.RichesUnlock:
            case ItemType.RichesRate:
                upgradeRiches++;
                break;
            case ItemType.MagnetUnlock:
            case ItemType.MagnetRate:
                upgradeMagnet++;
                break;
            case ItemType.SwordUnlock:
            case ItemType.SwordRate:
                upgradeSword++;
                break;
            case ItemType.PotionUnlock:
            case ItemType.PotionRate:
                upgradePotion++;
                break;
            case ItemType.CoinRate:
                upgradeCoin++;
                break;
            case ItemType.SpeedUp:
                upgradeSpeed++;
                break;
            case ItemType.HealthUp:
                upgradeHealth++;
                break;
            case ItemType.BoostInitial:
                upgradeBoostI++;
                break;
            case ItemType.BoostGain:
                upgradeBoostG++;
                break;
            case ItemType.BoostShield:
                upgradeBoostS++;
                break;
        }
    }
    // only used during Initialise()
    private void CalculateUpgradeEffectAll()
    {
        CalculateUpgradeEffect(ItemType.RushRate);
        CalculateUpgradeEffect(ItemType.RichesRate);
        CalculateUpgradeEffect(ItemType.MagnetRate);
        CalculateUpgradeEffect(ItemType.SwordRate);
        CalculateUpgradeEffect(ItemType.PotionRate);
        CalculateUpgradeEffect(ItemType.CoinRate);
        CalculateUpgradeEffect(ItemType.SpeedUp);
        CalculateUpgradeEffect(ItemType.HealthUp);
        CalculateUpgradeEffect(ItemType.BoostInitial);
        CalculateUpgradeEffect(ItemType.BoostGain);
        CalculateUpgradeEffect(ItemType.BoostShield);
    }
    private void CalculateUpgradeEffect(ItemType upgradeType)
    {
        switch (upgradeType)
        {
            case ItemType.RushUnlock:
            case ItemType.RushRate:
                if (upgradeRush > 0)
                    upgradeRushRate = rushRateBase + (rushRateBonus * upgradeRush);
                else
                    upgradeRushRate = 0;
                Debug.Log("upgradeRushRate " + upgradeRushRate);
                break;
            case ItemType.RichesUnlock:
            case ItemType.RichesRate:
                if (upgradeRiches > 0)
                    upgradeRichesRate = richesRateBase + (richesRateBonus * upgradeRiches);
                else
                    upgradeRichesRate = 0;
                Debug.Log("upgradeRichesRate " + upgradeRichesRate);
                break;
            case ItemType.MagnetUnlock:
            case ItemType.MagnetRate:
                if (upgradeMagnet > 0)
                    upgradeMagnetRate = magnetRateBase + (magnetRateBonus * upgradeMagnet);
                else
                    upgradeMagnetRate = 0;
                Debug.Log("upgradeMagnetRate " + upgradeMagnetRate);
                break;
            case ItemType.SwordUnlock:
            case ItemType.SwordRate:
                if (upgradeSword > 0)
                    upgradeSwordRate = swordRateBase + (swordRateBonus * upgradeSword);
                else
                    upgradeSwordRate = 0;
                Debug.Log("upgradeSwordRate " + upgradeSwordRate);
                break;
            case ItemType.PotionUnlock:
            case ItemType.PotionRate:
                if (upgradePotion > 0)
                    upgradePotionRate = potionRateBase + (potionRateBonus * upgradePotion);
                else
                    upgradePotionRate = 0;
                Debug.Log("upgradePotionRate " + upgradePotionRate);
                break;
            case ItemType.CoinRate:
                upgradeCoinRate = coinRateBase + (coinRateBonus * upgradeCoin);
                Debug.Log("upgradeCoinRate " + upgradeCoinRate);
                break;
            case ItemType.SpeedUp:
                // just a flat speed modifier
                upgradeSpeedScale = speedBase + (upgradeSpeed * speedBonus);
                Debug.Log("upgradeSpeedScale " + upgradeSpeedScale);
                break;
            case ItemType.HealthUp:
                upgradeHealthPoints = healthBase + (healthBonus * upgradeHealth);
                Debug.Log("upgradeHealthPoints " + upgradeHealthPoints);
                break;
            case ItemType.BoostInitial:
                upgradeBoostInitial = boostStartBase + (boostStartBonus * upgradeBoostI);
                Debug.Log("upgradeBoostInitial " + upgradeBoostInitial);
                break;
            case ItemType.BoostGain:
                upgradeBoostGainRate = boostGainBase + (boostGainBonus * upgradeBoostG);
                Debug.Log("upgradeBoostGainRate " + upgradeBoostGainRate);
                break;
            case ItemType.BoostShield:
                upgradeBoostShield = boostShieldBase + (boostShieldBonus * upgradeBoostS);
                Debug.Log("upgradeBoostShield " + upgradeBoostShield);
                break;
        }
    }

    // apply upgrades for a newly purchased item
    public void ApplyPurchaseUpgrade(SO_ShopItem itemNew)
    {
        ApplyItemUpgrade(itemNew);
        CalculateUpgradeEffect(itemNew.itemType);
    }
}
