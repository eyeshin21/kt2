using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class StarterPackMenu : UIMenu
{
    [SerializeField] eIAPKey keyIAP = eIAPKey.kStarterPack;

    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Show()
    {
        base.Show();

        txtPrice.text = string.Format("{0} {1}", CGTeamBridge.Instance.GetProductCurrencyFromStore(keyIAP), CGTeamBridge.Instance.GetProductPriceFromStore(keyIAP));

        EventManager.StartListening(EventVariables.BuyStarterPack, OnPurchase);
    }

    public override void Hide()
    {
        base.Hide();
    }

    void OnPurchase()
    {
        EconomyMenu.instance.AddGold(2400, iconCoin, null);
        UserConfig.Instance.AmountBoosterAddSlot += 1;
        UserConfig.Instance.AmountBoosterHandSelect += 1;
        UserConfig.Instance.AmountBoosterMagnet += 1;

        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.AddSlot);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.HandSelect);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Magnet);

        if (UIManager.Instance.mainMenu != null && UIManager.Instance.mainMenu.gameObject.activeInHierarchy)
        {
            UIManager.Instance.mainMenu.objBtnStarterPack.SetActive(false);
        }

        CGTeamBridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel, "soft", "coin", 2400, "purchase", "starter_pack_menu", UserConfig.Instance.Coin);

        Hide();
    }

    public void OnClickButtonBuy()
    {
        CGTeamBridge.Instance.Purchase(keyIAP);
    }
}
