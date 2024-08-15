using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// UIShopItem
// manages each shop item in the UI
// created 29/7/24
// modified 29/7/24

public class UIShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private Image imageBackground;
    [SerializeField] private Color imageBackgroundLocked = Color.grey;
    [SerializeField] private Color imageBackgroundBought = Color.green;
    [SerializeField] private Image imageItem;
    [SerializeField] private Image imagePriceType; // coin or gem
    [FormerlySerializedAs("imageStatus")][SerializeField] private Image imageLock;
    [SerializeField] private Sprite lockCoin;
    [SerializeField] private Sprite lockGem;
    [SerializeField] private TextMeshProUGUI textPrice;
    [SerializeField] private Button buttonBuy;
    [Header("Pops")]
    [SerializeField] private Transform popPos;
    [SerializeField] private UIPopMaker popMaker;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private UIPopMaker popMakerGem;
    [SerializeField] private AudioClip popSoundGem;
    private SO_ShopItem shopItem; // data for the actual item on this entry
    private UIShop ownerShop;

    public void Initialise(SO_ShopItem item, UIShop ownerShop)
    {
        shopItem = item;
        textName.text = item.shopViewName;
        imageItem.sprite = item.shopImage;
        if (item.costType == CostType.Coin)
        {
            imagePriceType.sprite = GameManager.instance.shopSettings.spriteCostCoin;
            textPrice.text = GlobalVars.DisplayCoins(item.costAmount);
        }
        else if (item.costType == CostType.Gem)
        {
            imagePriceType.sprite = GameManager.instance.shopSettings.spriteCostGem;
            textPrice.text = GlobalVars.DisplayGems(item.costAmount);
        }
        else
        {
            imagePriceType.enabled = false;
            textPrice.text = GlobalVars.DisplayPremium(item.costAmount);
        }
        this.ownerShop = ownerShop;
        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        if (shopItem)
        {
            ItemState itemState = UIShop.GetItemState(shopItem);

            switch (itemState)
            {
                case ItemState.Available:
                    bool affordable = UIShop.CanAfford(shopItem);
                    if (affordable)
                        buttonBuy.interactable = true;
                    else
                        buttonBuy.interactable = false;
                    imageBackground.color = Color.white;
                    imageLock.enabled = false;
                    gameObject.SetActive(true);
                    break;
                case ItemState.Visible:
                    gameObject.SetActive(true);
                    buttonBuy.interactable = false;
                    imageBackground.color = imageBackgroundLocked;
                    imageLock.enabled = true;
                    if (shopItem.costType == CostType.Coin)
                        imageLock.sprite = lockCoin;
                    else
                        imageLock.sprite = lockGem;
                    break;
                case ItemState.Hidden:
                    gameObject.SetActive(false);
                    break;
                case ItemState.Bought:
                    gameObject.SetActive(true);
                    buttonBuy.interactable = false;
                    imageBackground.color = imageBackgroundBought;
                    imageLock.enabled = false;
                    break;
            }
        }
        else
        {
            buttonBuy.interactable = false;
        }
    }

    // UI hook to buy this item
    public void ButtonBuy()
    {
        if (shopItem)
        {
            ItemState itemState = UIShop.GetItemState(shopItem);
            if (shopItem.costType == CostType.Premium)
            {
                ownerShop.BuyPremium(shopItem);
            }
            else if (itemState == ItemState.Available)
            {
                bool affordable = UIShop.CanAfford(shopItem);
                Vector3 pos = transform.position; // so we can force this item back into position after the shop rearranges
                if (affordable)
                {
                    if (ownerShop.BuyItem(shopItem))
                    {
                        Vector3 delta = transform.parent.position + pos - transform.position;
                        if (shopItem.costType == CostType.Coin)
                        {
                            popMaker.MakePops(shopItem.costAmount);
                            AudioManager.instance.SoundPlayVaried(popSound, popPos.position);
                        }
                        else
                        {
                            popMakerGem.MakePops(shopItem.costAmount);
                            AudioManager.instance.SoundPlayVaried(popSoundGem, popPos.position);
                        }
                        transform.parent.position = delta;
                    }
                }
            }
        }
    }
}
