using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SO_ShopItem
// SciptableObject for each item in the shop, storing icon and effect data
// created 29/7/24
// modified 29/7/24

public enum CostType
{
    Coin,
    Gem,
    Premium // costAmount is measured in ($: cents, £: pennies)
}

[CreateAssetMenu(fileName = "ShopItemNew", menuName = "ScriptableObjects/ShopItem", order = 2)]
public class SO_ShopItem : ScriptableObject
{
    public string shopUniqueName; // used for save data
    [Header("Shop data")]
    public string shopViewName; // just for display, change freely
    public Sprite shopImage;
    public SO_ShopItem[] shopDependency; // items which must be bought before this is unlocked
    public CostType costType;
    [Header("Premium costs are in cents/pennies/yen")]
    public int costAmount;
    [Header("Gameplay effects")]
    public ItemType itemType;
    public int itemMagnitude = 1; // most items are just x1, except for premium currency

}
