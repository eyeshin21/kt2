using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class RemoveAdsMenu : UIMenu
{
    [SerializeField] eIAPKey keyIAP = eIAPKey.kRemoveAdsBundle;

    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Show()
    {
        base.Show();

        txtPrice.text = string.Format("{0} {1}", CGTeamBridge.Instance.GetProductCurrencyFromStore(keyIAP), CGTeamBridge.Instance.GetProductPriceFromStore(keyIAP));

        EventManager.StartListening(EventVariables.BuyRemoveAds, OnPurchase);
    }

    public override void Hide()
    {
        base.Hide();
    }

    void OnPurchase()
    {
        EconomyMenu.instance.AddGold(2000, iconCoin, null);
        UserConfig.Instance.AmountBoosterAddSlot += 5;
        UserConfig.Instance.AmountBoosterHandSelect += 5;
        UserConfig.Instance.AmountBoosterMagnet += 5;

        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.AddSlot);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.HandSelect);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Magnet);

        if (UIManager.Instance.mainMenu != null && UIManager.Instance.mainMenu.gameObject.activeInHierarchy)
        {
            UIManager.Instance.mainMenu.objBtnRemoveAds.SetActive(false);
        }

        CGTeamBridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel, "soft", "coin", 2000, "purchase", "remove_ads_menu", UserConfig.Instance.Coin);

        Hide();
    }

    public void OnClickButtonBuy()
    {
        CGTeamBridge.Instance.Purchase(keyIAP);
    }
}
