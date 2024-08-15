using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIShop
// the actual shop manager
// created 29/7/24
// modified 30/7/24

public enum ItemState
{
    Bought,
    Available,
    Visible,
    Hidden
}

public class UIShop : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [Header("Main shop items")]
    [SerializeField] private GameObject shopItemBase;
    [SerializeField] private Transform shopItemHolder;
    [SerializeField] private UIShopItem shopItemOriginal;
    [SerializeField] private Scrollbar shopItemScroll;
    [SerializeField] private Button buttonTabItems;
    [Header("Gem shop items")]
    [SerializeField] private GameObject shopItemGemsBase;
    [SerializeField] private Transform shopItemGemsHolder;
    [SerializeField] private UIShopItem shopItemGemsOriginal;
    [SerializeField] private Scrollbar shopGemScroll;
    [SerializeField] private Button buttonTabGems;
    [SerializeField] private UIShopPremium shopPremium;
    private UIShopItem[] shopItems;
    private UIShopItem[] shopItemsGems;
    private UIMainMenu mainMenu;

    public void Initialise(UIMainMenu mainMenu)
    {
        this.mainMenu = mainMenu;
        // set up shop item data now that gamemanager instance exists
        if (GameManager.instance)
        {
            // set up main shop
            shopItems = new UIShopItem[GameManager.instance.shopSettings.shopItems.Length];
            for (int i = 0; i < shopItems.Length; i++)
            {
                if (i > 0)
                    shopItems[i] = Instantiate(shopItemOriginal, shopItemHolder);
                else
                    shopItems[i] = shopItemOriginal;
            }
            for (int i = 0; i < shopItems.Length; i++)
                shopItems[i].Initialise(GameManager.instance.shopSettings.shopItems[i], this);

            // set up gems shop
            shopItemsGems = new UIShopItem[GameManager.instance.shopSettings.shopItemsGems.Length];
            for (int i = 0; i < shopItemsGems.Length; i++)
            {
                if (i > 0)
                    shopItemsGems[i] = Instantiate(shopItemGemsOriginal, shopItemGemsHolder);
                else
                    shopItemsGems[i] = shopItemGemsOriginal;
            }
            for (int i = 0; i < shopItemsGems.Length; i++)
                shopItemsGems[i].Initialise(GameManager.instance.shopSettings.shopItemsGems[i], this);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            shopItemScroll.value = 0;
            shopGemScroll.value = 0;

            shopItemGemsBase.SetActive(false);
            buttonTabGems.interactable = true;
            buttonTabItems.interactable = false;
        }
        else
        {
            Debug.LogError("No Game manager!");
            shopItems = new UIShopItem[0];
            shopItemGemsBase.SetActive(false);
        }

        shopPremium.Initialise(this);
    }

    public static bool CanAfford(SO_ShopItem item)
    {
        if (item)
        {
            int playerStash;
            if (item.costType == CostType.Coin)
                playerStash = GameManager.instance.coinsStash;
            else
                playerStash = GameManager.instance.gemsStash;

            if (playerStash < item.costAmount)
                return false;
            else
                return true;
        }
        return false;
    }

    public static ItemState GetItemState(SO_ShopItem item)
    {
        bool locked = false;
        if (GameManager.instance.HasItem(item))
            return ItemState.Bought;

        for (int i = 0; i < item.shopDependency.Length; i++)
        {
            if (!GameManager.instance.HasItem(item.shopDependency[i]))
                locked = true;
        }
        if (locked)
        {
            bool hidden = false;
            // this item is locked, but if all of it's dependencies are at least available (if not owned) then this item will be visible
            for (int i = 0; i < item.shopDependency.Length; i++)
            {
                ItemState dependencyState = GetItemState(item.shopDependency[i]);
                if (dependencyState == ItemState.Visible || dependencyState == ItemState.Hidden)
                    hidden = true;
            }
            if (hidden)
                return ItemState.Hidden;
            else
                return ItemState.Visible;
        }
        else
        {
            return ItemState.Available;
        }
    }

    public bool BuyItem(SO_ShopItem item)
    {
        if (GameManager.instance.GetItem(item))
        {
            AudioManager.instance.SoundPlayMenuButton();
            UpdateShopItems();
            if (mainMenu)
                mainMenu.UpdateScores();
            return true;
        }
        return false;
    }

    private void UpdateShopItems()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].UpdateAvailability();
        }
    }

    public void BuyPremium(SO_ShopItem item)
    {
        AudioManager.instance.SoundPlayMenuButton();
        shopPremium.OpenPremiumShop(item);
    }
    public void BuyPremiumConfirm(SO_ShopItem item)
    {
        // do lots of pops and flashes and shinies!!!
        if (mainMenu)
            mainMenu.UpdateScores();
    }
    public void BuyPremiumCancel()
    {
        // juse close :(((
        AudioManager.instance.SoundPlayMenuButton();
    }

    // ui button hooks
    public void ButtonTabItems()
    {
        mainMenu.SoundButton();
        shopItemBase.SetActive(true);
        shopItemGemsBase.SetActive(false);
        buttonTabGems.interactable = true;
        buttonTabItems.interactable = false;
        shopItemScroll.value = 0;
    }
    public void ButtonTabGems()
    {
        mainMenu.SoundButton();
        shopItemBase.SetActive(false);
        shopItemGemsBase.SetActive(true);
        buttonTabGems.interactable = false;
        buttonTabItems.interactable = true;
        shopGemScroll.value = 0;
    }
    public void ButtonCloseShop()
    {
        mainMenu.ButtonBack();
    }
}
