using DG.Tweening;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutOfSpaceMenu : UIMenu
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform rtsfHoldToSee;
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] TextMeshProUGUI txtPriceRevive;
    [SerializeField] int priceRevive = 120;
    [SerializeField] GameObject objButtonWatchAds;

    [Header("Starter Pack")]
    [SerializeField] GameObject objStarterPack;
    [SerializeField] eIAPKey keyIAP = eIAPKey.kStarterPack;
    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Show()
    {
        base.Show();

        txtPriceRevive.text = priceRevive.ToString();

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        icon.SetNativeSize();

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        canvasGroup.DOKill();
        canvasGroup.alpha = 1f;

        objButtonWatchAds.SetActive(UserConfig.Instance.CurLevel > 15);

        //if (CGTeamBridge.Instance.IsNonConsumablePurchased(eIAPKey.kStarterPack))
        //{
        rtsfHoldToSee.anchoredPosition = Vector2.up * -350f;
        objStarterPack.SetActive(false);
        //}
        //else
        //{
        //    rtsfHoldToSee.anchoredPosition = Vector2.up * -788f;
        //    objStarterPack.SetActive(true);

        //    objStarterPack.transform.localScale = Vector3.zero;
        //    objStarterPack.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        //    txtPrice.text = string.Format("{0} {1}", CGTeamBridge.Instance.GetProductCurrencyFromStore(keyIAP), CGTeamBridge.Instance.GetProductPriceFromStore(keyIAP));

        //    EventManager.StartListening(EventVariables.BuyStarterPack, OnPurchase);
        //}
    }

    public override void Hide()
    {
        base.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        objStarterPack.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        EventManager.StopListening(EventVariables.BuyStarterPack, OnPurchase);
    }

    public void PressedBackBtn()
    {
        base.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            UIManager.Instance.ShowLoseMenu();
        });

        objStarterPack.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        EventManager.StopListening(EventVariables.BuyStarterPack, OnPurchase);
    }

    public void PressedKeepPlayingBtn()
    {
        if (UserConfig.Instance.Coin >= priceRevive)
        {
            EconomyMenu.instance.AddGold(-priceRevive, null, null);
            GameplayController.Instance.Revive();

            CGTeamBridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "soft", "coin", priceRevive, "buy_revive", "revive_menu", UserConfig.Instance.Coin);
            Hide();
        }
        else
        {
            UIManager.Instance.ShowShopMenu();
        }
    }

    public void PressedWatchAdsBtn()
    {
        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            SoundManager.instance.PlaySound("GetReward");
            GameplayController.Instance.Revive();
            Hide();
        });
        CGTeamBridge.Instance.ShowRewarded("revive", null, e, null);
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

        GameplayController.Instance.Revive();

        Hide();
    }

    public void OnClickButtonBuy()
    {
        CGTeamBridge.Instance.Purchase(keyIAP);
    }

    public void OnPointerDown()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.Linear);
    }

    public void OnPointerUp()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear);
    }
}
