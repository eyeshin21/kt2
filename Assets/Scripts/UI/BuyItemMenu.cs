using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyItemMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;

    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI contentTxt;

    [SerializeField] Image[] iconImg;

    [SerializeField] Sprite[] iconItem;

    [SerializeField] TextMeshProUGUI priceTxt;
    [SerializeField] int[] prices;

    [HideInInspector]
    public eBooster eBooster;

    public override void Show()
    {
        base.Show();

        //Bridge.Instance.TrackCustomEvent("show_buy_booster", UserConfig.Instance.CurLevel);
        switch (eBooster)
        {
            case eBooster.AddSlot:
                titleTxt.text = "Add Slot";
                contentTxt.text = "Gain a temporary slot for one hook.";
                break;
            case eBooster.HandSelect:
                titleTxt.text = "Hand Select";
                contentTxt.text = "Pick any hooks from the queue.";
                break;
            case eBooster.Magnet:
                titleTxt.text = "Magnet";
                contentTxt.text = "Pick any hooks from the slot to shoot.";
                break;
        }

        priceTxt.text = prices[(int)eBooster].ToString();

        for (int i = 0; i < iconImg.Length; i++)
        {
            iconImg[i].sprite = iconItem[(int)eBooster];
            //iconImg.SetNativeSize();
        }

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void PressedBuyRewardBtn()
    {
        string placement = "";
        switch (eBooster)
        {
            case eBooster.AddSlot:
                placement = "add_slot";
                break;
            case eBooster.HandSelect:
                placement = "hand_select";
                break;
            case eBooster.Magnet:
                placement = "magnet";
                break;
        }

        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            SoundManager.instance.PlaySound("GetReward");
            switch (eBooster)
            {
                case eBooster.AddSlot:
                    UserConfig.Instance.AmountBoosterAddSlot += 1;
                    CGTeamBridge.Instance.TrackBuyBooster("add_slot", UserConfig.Instance.CurLevel);
                    break;
                case eBooster.HandSelect:
                    UserConfig.Instance.AmountBoosterHandSelect += 1;
                    CGTeamBridge.Instance.TrackBuyBooster("hand_select", UserConfig.Instance.CurLevel);
                    break;
                case eBooster.Magnet:
                    UserConfig.Instance.AmountBoosterMagnet += 1;
                    CGTeamBridge.Instance.TrackBuyBooster("magnet", UserConfig.Instance.CurLevel);
                    break;
            }
            EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);
            Hide();
        });
        CGTeamBridge.Instance.ShowRewarded(placement, null, e, null);
    }

    public void PressedBuyCoinBtn()
    {
        int price = prices[(int)eBooster];
        if (UserConfig.Instance.Coin >= price)
        {
            EconomyMenu.instance.AddGold(-price, null, null);
            switch (eBooster)
            {
                case eBooster.AddSlot:
                    UserConfig.Instance.AmountBoosterAddSlot += 2;
                    CGTeamBridge.Instance.TrackBuyBooster("add_slot", UserConfig.Instance.CurLevel);
                    break;
                case eBooster.HandSelect:
                    UserConfig.Instance.AmountBoosterHandSelect += 2;
                    CGTeamBridge.Instance.TrackBuyBooster("hand_select", UserConfig.Instance.CurLevel);
                    break;
                case eBooster.Magnet:
                    UserConfig.Instance.AmountBoosterMagnet += 2;
                    CGTeamBridge.Instance.TrackBuyBooster("magnet", UserConfig.Instance.CurLevel);
                    break;
            }
            CGTeamBridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "soft", "coin", price, "buy_booster", "buy_item_menu", UserConfig.Instance.Coin);
            EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);
            Hide();
        }
        else
        {
            UIManager.Instance.ShowShopMenu();
        }
    }

    public override void Hide()
    {
        base.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
