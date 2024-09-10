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
    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private RectTransform buttonPlayRect;
    [Header("Main shop items")]
    [SerializeField] private GameObject shopItemBase;
    [SerializeField] private ScrollRect shopItemScrollRect;
    [SerializeField] private Transform shopItemHolder;
    [SerializeField] private UIShopItem shopItemOriginal;
    [SerializeField] private Scrollbar shopItemScroll;
    [SerializeField] private Button buttonTabItems;
    [Header("Gem shop items")]
    [SerializeField] private GameObject shopItemGemsBase;
    [SerializeField] private ScrollRect shopGemScrollRect;
    [SerializeField] private Transform shopItemGemsHolder;
    [SerializeField] private UIShopItem shopItemGemsOriginal;
    [SerializeField] private Scrollbar shopGemScroll;
    [SerializeField] private Button buttonTabGems;
    [SerializeField] private UIShopPremium shopPremium;
    private UIShopItem[] shopItems;
    private bool shopItemsFirstOpen = true;
    private UIShopItem[] shopItemsGems;
    private bool shopGemsFirstOpen = true;
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

            //LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            //shopItemScroll.value = 0;
            //shopGemScroll.value = 0;

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

        if (GameManager.PlayModeElseMenu())
        {
            buttonBack.gameObject.SetActive(false);
        }

        shopPremium.Initialise(this);
    }

    public static bool CanAfford(SO_ShopItem item)
    {
        if (item)
        {
            if (item.costType == CostType.Premium)
                return true;

            int playerStash;
            if (item.costType == CostType.Coin)
                playerStash = GameManager.instance.coinsStash;
            else
                playerStash = GameManager.instance.gemsStash;


            Debug.Log("CanAfford with item " + item.shopViewName + " type " + item.costType + " player has " + playerStash + " of the right type against cost of " + item.costAmount);

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

        if (item.costType == CostType.Premium)
            return ItemState.Available;


        if (GameManager.instance.HasItem(item))
            return ItemState.Bought;

        for (int i = 0; i < item.shopDependency.Length; i++)
        {
            if (!GameManager.instance.HasItem(item.shopDependency[i]))
            {
                if (item.shopDependency[i].tutorialBlackout)
                    return ItemState.Hidden;

                locked = true;
            }
        }
        if (locked)
        {
            // this item is locked, but if all of it's dependencies are at least available (if not owned) then this item will be visible
            for (int i = 0; i < item.shopDependency.Length; i++)
            {
                ItemState dependencyState = GetItemState(item.shopDependency[i]);
                if (dependencyState == ItemState.Visible || dependencyState == ItemState.Hidden)
                    return ItemState.Hidden;
            }
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
            if (mainMenu && !GameManager.PlayModeElseMenu())
                mainMenu.UpdateScores();
            else
                PlayerPawn.instance.pawnPurse.ChangeBars();
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

    public bool IsBuyAvailable()
    {
        for (int i = 0; i < GameManager.instance.shopSettings.shopItems.Length; i++)
        {
            if (GameManager.instance.shopSettings.shopItems[i].costType == CostType.Premium)
                continue;
            if (GetItemState(GameManager.instance.shopSettings.shopItems[i]) == ItemState.Available)
            {
                if (CanAfford(GameManager.instance.shopSettings.shopItems[i]))
                    return true;
            }
        }
        return false;
    }

    public void BuyPremium(SO_ShopItem item)
    {
        AudioManager.instance.SoundPlayMenuButton();
        shopPremium.OpenPremiumShop(item);
    }
    public void BuyPremiumConfirm(SO_ShopItem item)
    {
        // do lots of pops and flashes and shinies!!!
        if (mainMenu && !GameManager.PlayModeElseMenu())
            mainMenu.UpdateScores();
        else
            PlayerPawn.instance.pawnPurse.ChangeBars();
        UpdateShopItems();
    }
    public void BuyPremiumCancel()
    {
        // juse close :(((
        AudioManager.instance.SoundPlayMenuButton();
    }

    // ui button hooks
    public void ButtonTabItems()
    {
        //Debug.Log("Line 222");
        mainMenu.SoundButton();
        //Debug.Log("Line 224");
        shopItemBase.SetActive(true);
        //Debug.Log("Line 226");
        shopItemGemsBase.SetActive(false);
        //Debug.Log("Line 228");
        buttonTabGems.interactable = true;
        //Debug.Log("Line 230");
        buttonTabItems.interactable = false;
        //Debug.Log("Line 232");
        //shopItemScroll.value = 0;
        if (shopItemsFirstOpen)
        {
            shopItemsFirstOpen = false;
            StartCoroutine(ScrollToObject(shopItemScrollRect, shopItems[0].rectTransform));
            //Debug.Log("Line 238");
        }
        //Debug.Log("Line 240");
        UpdateShopItems();
        //Debug.Log("Line 242");
    }
    public void ButtonTabGems()
    {
        mainMenu.SoundButton();
        shopItemBase.SetActive(false);
        shopItemGemsBase.SetActive(true);
        buttonTabGems.interactable = false;
        buttonTabItems.interactable = true;
        //shopGemScroll.value = 0;
        if (shopGemsFirstOpen)
        {
            shopGemsFirstOpen = false;
            StartCoroutine(ScrollToObject(shopGemScrollRect, shopItemsGems[0].rectTransform));
        }
    }
    public void ButtonCloseShop()
    {
        mainMenu.ButtonBack();
    }

    public void ButtonPlayShop()
    {
        if (GameManager.PlayModeElseMenu())
        {
            // play mode, trigger a new run
            GameManager.instance.SaveSettings();
            TerrainManager.instance.RestartStage();
        }
        else
        {
            // menu mode, go to play mode
            mainMenu.ButtonPlay();
        }
    }

    public void OpenShopTutorial()
    {
        buttonBack.interactable = false;
        buttonPlay.interactable = false;
        buttonTabItems.gameObject.SetActive(false);
        buttonTabGems.gameObject.SetActive(false);
        // TODO make the available item sparkle
        for (int i = 0; i < GameManager.instance.shopSettings.shopItems.Length; i++)
        {
            // there should only be one item available at this time, so take the first one found
            if (GetItemState(GameManager.instance.shopSettings.shopItems[i]) == ItemState.Available)
            {
                shopItems[i].SparkleButton();
                break;
            }
        }
    }
    public void ShopTutorialPrompt()
    {
        buttonPlay.interactable = true;
        UIPopManager.instance.StartPopRect(buttonPlayRect, Color.white, true);
    }
    public void ShopTutorialFinished()
    {
        buttonBack.interactable = true;
        buttonPlay.interactable = true;
        UIPopManager.instance.StopPopRect();
        buttonTabItems.gameObject.SetActive(true);
        buttonTabGems.gameObject.SetActive(true);
    }

    private IEnumerator ScrollToObject(ScrollRect scrollRect, RectTransform targetObject)
    {
        yield return new WaitForEndOfFrame();
        scrollRect.content.localPosition = scrollRect.GetSnapToPositionToBringChildIntoView(targetObject);
    }
}
