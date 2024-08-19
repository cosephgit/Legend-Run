using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UIShopPremium
// just a placeholder at the moment, for actual currency shop implementation
// created 15/8/24
// modified 15/8/24

public class UIShopPremium : MonoBehaviour
{
    [SerializeField] private Image imageGem;
    [SerializeField] private TextMeshProUGUI buttonBuyText;
    [SerializeField] private TextMeshProUGUI buttonGemText;
    [SerializeField] private UIPopMaker buyConfirmPops;
    [SerializeField] private AudioClip buyConfirmSound;
    private UIShop shopMaster;
    private SO_ShopItem itemView;

    public void Initialise(UIShop shopMaster)
    {
        this.shopMaster = shopMaster;
        gameObject.SetActive(false);
    }

    public void OpenPremiumShop(SO_ShopItem item)
    {
        itemView = item;
        imageGem.sprite = item.shopImage;
        buttonGemText.text = item.shopViewName;
        buttonBuyText.text = "Pay " + GlobalVars.DisplayPremium(item.costAmount);
        gameObject.SetActive(true);
    }

    public void ButtonPremiumConfirm()
    {
        // do lots of pops and flashes and shinies!!!
        if (GameManager.instance.GetItemPremium(itemView))
        {
            buyConfirmPops.MakePops(itemView.costAmount);
            AudioManager.instance.SoundPlayVaried(buyConfirmSound, Vector2.zero);


            shopMaster.BuyPremiumConfirm(itemView);
            gameObject.SetActive(false);
        }
    }

    public void ButtonPremiumClose()
    {
        shopMaster.BuyPremiumCancel();
        gameObject.SetActive(false);
    }
}
