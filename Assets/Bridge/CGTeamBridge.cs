using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CGTeamBridge : MonoBehaviour
{
    public static CGTeamBridge Instance;

    public int RetendDay
    {
        get
        {
            return PlayerPrefs.GetInt("retendDay", 0);
        }
        set
        {
            PlayerPrefs.SetInt("retendDay", value);
        }
    }

    public int DayPlayed
    {
        get
        {
            return PlayerPrefs.GetInt("dayPlayed", 0);
        }
        set
        {
            PlayerPrefs.SetInt("dayPlayed", value);
        }
    }

    public int DailyLogin
    {
        get
        {
            return dailyLogin;
        }
        set
        {
            dailyLogin = value;
            PlayerPrefs.SetInt("dailyLogin", value);
        }
    }

    private int dailyLogin;
    private bool canShowDailyBonus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        CheckDailyLogin();
    }

    public bool HasInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }

    #region ADS
    public void ShowMaxDebugger()
    {

    }

    public void ShowBanner()
    {

    }

    public void HideBanner()
    {

    }

    public void ShowInterstitial(string placement, UnityEvent onClose)
    {
        onClose?.Invoke();
    }

    public void ShowRewarded(string placement, UnityEvent onStart, UnityEvent onCompleted, UnityEvent onFailed)
    {
        onCompleted?.Invoke();
    }

    public bool IsRewardReady()
    {
        return true;
    }
    #endregion

    #region Analytics
    void LogEvent(string eventName, Dictionary<string, object> parameters)
    {
        Debug.Log("Firebase Analytics: " + eventName);
    }

    public void TrackCustomEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (parameters != null)
        {
            foreach (KeyValuePair<string, object> kv in parameters)
            {
                Debug.Log(string.Format("FirebaseAnalytics: {0} - Parameters: {1} -- {2}", eventName, kv.Key, kv.Value));
            }
        }
        LogEvent(eventName, parameters);
    }

    public void TrackTutAction(string action_name)
    {
        var parameters = new Dictionary<string, object>
            {
                {"action_name", action_name}
            };
        TrackCustomEvent("tut_action", parameters);
    }

    /// <summary>
    /// Log khi vừa bắt đầu level
    /// </summary>
    /// <param name="level">level đang chơi</param> 
    /// <param name="difficult">độ khó level</param> 
    public void OnGameStarted(int level, string difficult)
    {

    }

    /// <summary>
    /// Log khi kết thúc game. Bao gồm: win, lose
    /// </summary>
    /// <param name="levelComplete">trạng thái game thắng hay thua</param> 
    /// <param name="mission_progress">điểm của level. Số khay còn lại trong level. nếu win thì là 1, nếu thua thì thua khi giải được bao nhiêu phần của level</param> 
    /// <param name="level">level đang chơi</param> 
    /// <param name="coinEarn">số coin kiếm được</param> 
    public void OnGameFinished(bool levelComplete, float mission_progress, int level, int coinEarn = 0)
    {

    }

    /// <summary>
    /// Log khi user thoát ra home hoặc ấn replay lại lúc đang chơi
    /// </summary>
    /// <param name="level">level đang chơi</param> 
    /// <param name="mission_progress">điểm của level</param> 
    public void OnGameAbandoned(int level, float mission_progress)
    {

    }

    /// <summary>
    /// Log khi xảy ra các sự kiện trong game. Hiện tại chỉ cần log khi hiện revive và sau khi revive là được
    /// </summary>
    /// <param name="level">level đang chơi</param> 
    /// <param name="mission_progress">điểm của level</param> 
    /// <param name="stepName">Nếu hiện revive thì stepName = "soft_fail", còn nếu sau khi revive thì stepName = "revive"</param> 
    public void OnGameStep(int level, float mission_progress, string stepName)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level">level đang chơi</param> 
    /// <param name="item_type">Coin = soft, Gem = hard, Booster = power_up</param> 
    /// <param name="name">coin, gem, tên các booster,...</param> 
    /// <param name="amount">Số lượng tiêu</param> 
    /// <param name="reason">Lý do tiêu</param> 
    /// <param name="screen">màn hình thực hiện hành động</param> 
    /// <param name="balance">Số lượng sau khi thực hiện hành động</param> 
    public void LogSpendResource(int level, string item_type, string name, int amount, string reason, string screen, int balance)
    {

    }

    /// <summary>
    /// Log khi user ấn mua booster
    /// </summary>
    /// <param name="nameBooster">tên của loại booster</param> 
    /// <param name="level">level đang chơi</param> 
    public void TrackBuyBooster(string nameBooster, int level)
    {

    }

    /// <summary>
    /// Log khi user dùng booster
    /// </summary>
    /// <param name="nameBooster">tên của loại booster</param> 
    /// <param name="level">level đang chơi</param> 
    public void TrackUseBooster(string nameBooster, int level)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level">level đang chơi</param> 
    /// <param name="item_type">Coin = soft, Gem = hard, Booster = power_up</param> 
    /// <param name="name">coin, gem, tên các booster,...</param> 
    /// <param name="amount">Số lượng kiếm được</param> 
    /// <param name="reason">Lý do kiếm được</param> 
    /// <param name="screen">Màn hình thực hiện hành động</param> 
    /// <param name="balance">Số lượng sau khi thực hiện hành động</param> 
    public void LogEarnCurrency(int level, string item_type, string name, int amount, string reason, string screen, int balance)
    {
       
    }

    void SetUserProperty(string name, string value)
    {
        Debug.Log(name + ": " + value);
    }

    public void SetLevelProperty(int level)
    {
        SetUserProperty("current_level", level.ToString());
    }
    #endregion

    #region Remote Config
    public int GetScoreRate()
    {
        return 5;
    }

    public int GetShowRateFrequency()
    {
        return 10;
    }
    #endregion

    #region Social
    [Header("Social")]
    private const string AndroidRatingURI = "market://details?id={0}";
    private const string IOSRatingURI = "https://apps.apple.com/app/id{0}";
    string url = string.Empty;
    [SerializeField]
    string STORE_APP_ID = string.Empty;
    public void RateGame()
    {
        Debug.Log("Rate Game");
    }
    #endregion

    #region IAP
    public void Purchase(eIAPKey eIAPKey)
    {
        Debug.Log("Purchase: " + eIAPKey);
        switch (eIAPKey)
        {
            case eIAPKey.kRemoveAdsBundle:
                IAPHandlePurchase.instance.BuyRemoveAdsBundle();
                break;
            case eIAPKey.kStarterPack:
                IAPHandlePurchase.instance.BuyStarterPackBundle();
                break;
            case eIAPKey.kCoin1:
                IAPHandlePurchase.instance.BuyCoin1();
                break;
            case eIAPKey.kCoin2:
                IAPHandlePurchase.instance.BuyCoin2();
                break;
            case eIAPKey.kCoin3:
                IAPHandlePurchase.instance.BuyCoin3();
                break;
            case eIAPKey.kCoin4:
                IAPHandlePurchase.instance.BuyCoin4();
                break;
            case eIAPKey.kCoin5:
                IAPHandlePurchase.instance.BuyCoin5();
                break;
            case eIAPKey.kCoin6:
                IAPHandlePurchase.instance.BuyCoin6();
                break;
        }
    }

    public void RestorePurchase()
    {
        Debug.Log("Restore Purchase");
    }

    public string GetProductCurrencyFromStore(eIAPKey eIAPKey)
    {
        return "$";
    }

    public string GetProductPriceStringFromStore(eIAPKey eIAPKey)
    {
        return "0";
    }

    public decimal GetProductPriceFromStore(eIAPKey eIAPKey)
    {
        return 0;
    }

    public bool IsNonConsumablePurchased(eIAPKey eIAPKey)
    {
        return PlayerPrefs.HasKey(eIAPKey.ToString());
    }
    #endregion

    #region Daily Login
    void CheckDailyLogin()
    {
        if (DailyLogin == 0)
        {
            PlayerPrefs.SetInt("retent_type", 0);

            canShowDailyBonus = true;
            return;
        }

        System.DateTime currentTimeDate = System.DateTime.Now.Date;
        System.DateTime lastTimeOpen = System.DateTime.Parse(PlayerPrefs.GetString("lastTimeOpen", System.DateTime.Now.ToString()));
        Debug.Log(PlayerPrefs.GetString("lastTimeOpen", System.DateTime.Now.ToString()));

        int dayOpen = (currentTimeDate - lastTimeOpen).Days;
        if (dayOpen > 0)
        {
            int retend_type = PlayerPrefs.GetInt("retent_type");
            retend_type += dayOpen;
            PlayerPrefs.SetInt("retent_type", retend_type);
            canShowDailyBonus = true;
        }
    }

    public bool CanShowDailyBonus()
    {
        return canShowDailyBonus;
    }

    public void DailyLoginSuccessful()
    {
        DailyLogin++;
        PlayerPrefs.SetString("lastTimeOpen", System.DateTime.Now.Date.ToString());
    }
    #endregion
}