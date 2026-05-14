using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum eBooster
{
    HandSelect = 0,
    AddSlot = 1,
    Magnet = 2
}

public class IngameMenu : UIMenu
{
    public TutorialUI tutorialUI;

    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] GameObject booster;
    [SerializeField] Transform trsfBoard;
    [SerializeField] Transform trsfBottom;

    [Header("Booster Hand Select")]
    [SerializeField] AnimatedButton boosterHandSelectBtn;
    [SerializeField] TextMeshProUGUI amountBoosterHandSelectTxt;
    [SerializeField] GameObject iconAddBoosterHandSelect;
    [SerializeField] GameObject lockBoosterHandSelect;
    [SerializeField] GameObject unlockBoosterHandSelect;
    [SerializeField] TextMeshProUGUI levelUnlockBoosterHandSelectTxt;
    [SerializeField] GameObject[] blocksHandSelect;

    [Header("Booster Add Slot")]
    [SerializeField] AnimatedButton boosterAddSlotBtn;
    [SerializeField] TextMeshProUGUI amountBoosterAddSlotTxt;
    [SerializeField] GameObject iconAddBoosterAddSlot;
    [SerializeField] GameObject lockBoosterAddSlot;
    [SerializeField] GameObject unlockBoosterAddSlot;
    [SerializeField] TextMeshProUGUI levelUnlockBoosterAddSlotTxt;
    [SerializeField] GameObject[] blocksAddSlot;

    [Header("Booster Magnet")]
    [SerializeField] AnimatedButton boosterMagnetBtn;
    [SerializeField] TextMeshProUGUI amountBoosterMagnetTxt;
    [SerializeField] GameObject iconAddBoosterMagnet;
    [SerializeField] GameObject lockBoosterMagnet;
    [SerializeField] GameObject unlockBoosterMagnet;
    [SerializeField] TextMeshProUGUI levelUnlockBoosterMagnetTxt;
    [SerializeField] GameObject[] blocksMagnet;

    bool canControlUI = true;

    public override void Start()
    {
        base.Start();

        EventManager.StartListening(EventVariables.EndGame, EndGame);
        EventManager.StartListening(EventVariables.LoadLevel, LoadLevel);
        EventManager.StartListening(EventVariables.AddItem, AddItem);
        EventManager.StartListening(EventVariables.Revive, Revive);

        levelUnlockBoosterHandSelectTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT);
        levelUnlockBoosterAddSlotTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT);
        levelUnlockBoosterMagnetTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET);
    }

    public override void Show()
    {
        base.Show();

        if (!content.activeSelf)
        {
            ActiveUI(true);
        }

        //booster.SetActive(UserConfig.Instance.CurLevel != 1);
        booster.SetActive(false);

        SetStateBoosterHandSelect();
        SetStateBoosterAddSlot();
        SetStateBoosterMagnet();

        levelTxt.text = $"Level {UserConfig.Instance.CurLevel.ToString()}";

        //if (UserConfig.Instance.HasTurnOffInternet())
        //{
        //    UIManager.Instance.ShowNoticeMenu("Internet Connection Error", true);
        //}

        UserConfig.Instance.amountPlay++;
    }

    void Revive()
    {
        GameManager.Instance.canControl = true;
        canControlUI = true;

        int totalItems = 0;
        int cleardItems = 0;
        float progress = cleardItems / (float)totalItems;
        CGTeamBridge.Instance.OnGameStep(UserConfig.Instance.CurLevel, progress, "revive");
    }

    void LoadLevel()
    {
        canControlUI = true;
    }

    void EndGame()
    {
        canControlUI = false;
    }

    void AddItem()
    {
        int idItem = EventManager.GetInt(EventVariables.AddItem);
        switch ((eBooster)idItem)
        {
            case eBooster.HandSelect:
                SetStateBoosterHandSelect();
                break;
            case eBooster.AddSlot:
                SetStateBoosterAddSlot();
                break;
            case eBooster.Magnet:
                SetStateBoosterMagnet();
                break;
        }
    }

    void SetStateBoosterHandSelect()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT)
        {
            unlockBoosterHandSelect.SetActive(true);
            lockBoosterHandSelect.SetActive(false);

            if (UserConfig.Instance.AmountBoosterHandSelect > 0)
            {
                amountBoosterHandSelectTxt.transform.parent.gameObject.SetActive(true);
                amountBoosterHandSelectTxt.text = UserConfig.Instance.AmountBoosterHandSelect.ToString();
                iconAddBoosterHandSelect.SetActive(false);
            }
            else
            {
                amountBoosterHandSelectTxt.transform.parent.gameObject.SetActive(false);
                iconAddBoosterHandSelect.SetActive(true);
            }
        }
        else
        {
            unlockBoosterHandSelect.SetActive(false);
            lockBoosterHandSelect.SetActive(true);
        }
    }

    void SetStateBoosterAddSlot()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT)
        {
            unlockBoosterAddSlot.SetActive(true);
            lockBoosterAddSlot.SetActive(false);

            if (UserConfig.Instance.AmountBoosterAddSlot > 0)
            {
                amountBoosterAddSlotTxt.transform.parent.gameObject.SetActive(true);
                amountBoosterAddSlotTxt.text = UserConfig.Instance.AmountBoosterAddSlot.ToString();
                iconAddBoosterAddSlot.SetActive(false);
            }
            else
            {
                amountBoosterAddSlotTxt.transform.parent.gameObject.SetActive(false);
                iconAddBoosterAddSlot.SetActive(true);
            }
        }
        else
        {
            unlockBoosterAddSlot.SetActive(false);
            lockBoosterAddSlot.SetActive(true);
        }
    }

    void SetStateBoosterMagnet()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
        {
            unlockBoosterMagnet.SetActive(true);
            lockBoosterMagnet.SetActive(false);

            if (UserConfig.Instance.AmountBoosterMagnet > 0)
            {
                amountBoosterMagnetTxt.transform.parent.gameObject.SetActive(true);
                amountBoosterMagnetTxt.text = UserConfig.Instance.AmountBoosterMagnet.ToString();
                iconAddBoosterMagnet.SetActive(false);
            }
            else
            {
                amountBoosterMagnetTxt.transform.parent.gameObject.SetActive(false);
                iconAddBoosterMagnet.SetActive(true);
            }
        }
        else
        {
            unlockBoosterMagnet.SetActive(false);
            lockBoosterMagnet.SetActive(true);
        }
    }

    public void ActiveBoosterHandSelect(bool active)
    {
        if (active)
        {
            boosterHandSelectBtn.interactable = true;
            for (int i = 0; i < blocksHandSelect.Length; i++)
            {
                blocksHandSelect[i].SetActive(false);
            }
        }
        else
        {
            boosterHandSelectBtn.interactable = false;
            for (int i = 0; i < blocksHandSelect.Length; i++)
            {
                blocksHandSelect[i].SetActive(true);
            }
        }
    }

    public void ActiveBoosterAddSlot(bool active)
    {
        if (active)
        {
            boosterAddSlotBtn.interactable = true;
            for (int i = 0; i < blocksAddSlot.Length; i++)
            {
                blocksAddSlot[i].SetActive(false);
            }
        }
        else
        {
            boosterAddSlotBtn.interactable = false;
            for (int i = 0; i < blocksAddSlot.Length; i++)
            {
                blocksAddSlot[i].SetActive(true);
            }
        }
    }

    public void ActiveBoosterMagnet(bool active)
    {
        if (active)
        {
            boosterMagnetBtn.interactable = true;
            for (int i = 0; i < blocksMagnet.Length; i++)
            {
                blocksMagnet[i].SetActive(false);
            }
        }
        else
        {
            boosterMagnetBtn.interactable = false;
            for (int i = 0; i < blocksMagnet.Length; i++)
            {
                blocksMagnet[i].SetActive(true);
            }
        }
    }

    public void PressedSettingBtn()
    {
        if (canControlUI && BoxManager.Instance.boxes.Count > 0)
        {
            UIManager.Instance.ShowSettingMenu();
        }
    }

    public void PressedReplayBtn()
    {
        ReplayLevel();
    }

    public void ReplayLevel()
    {
        if (canControlUI)
        {
            SoundManager.instance.StopSoundMedium();
            canControlUI = false;
            EventManager.EmitEvent(EventVariables.RecycleLevel);

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                GameManager.Instance.LoadLevel();
                Show();
            });
            FadeMenu.Instance.Fade(e, true);

            int totalItems = 0;
            int cleardItems = 0;
            float progress = cleardItems / (float)totalItems;
            CGTeamBridge.Instance.OnGameAbandoned(UserConfig.Instance.CurLevel, progress);
        }
    }

    public void PressedBoosterHandSelectBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT)
            {
                if (UserConfig.Instance.AmountBoosterHandSelect > 0)
                {
                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT)
                    {
                        tutorialUI.Hide();
                    }

                    UIManager.Instance.ShowItemUsingMenu(eBooster.HandSelect);
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.HandSelect);
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_HAND_SELECT));
            }
        }
    }

    public void PressedBoosterAddSlotBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT)
            {
                if (UserConfig.Instance.AmountBoosterAddSlot > 0)
                {
                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT)
                    {
                        tutorialUI.Hide();
                    }

                    //if (ParkingManager.Instance.AddSlot())
                    //{
                    //    UserConfig.Instance.AmountBoosterAddSlot--;
                    //    SetStateBoosterAddSlot();

                    //    CGTeamBridge.Instance.TrackUseBooster("add_slot", UserConfig.Instance.CurLevel);
                    //}
                    //else
                    //{
                    //    if (ParkingManager.Instance.TotalParkedBox() == 0)
                    //    {
                    //        WarningMenu.Instance.Show(Vector2.zero, "Slots are empty!");
                    //    }
                    //    else if (ParkingManager.Instance.IsExtraParkingSlotFull())
                    //    {
                    //        WarningMenu.Instance.Show(Vector2.zero, "Temporary slots are full!");
                    //    }
                    //    else
                    //    {
                    //        WarningMenu.Instance.Show(Vector2.zero, "Can't add slot!");
                    //    }
                    //}
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.AddSlot);
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_ADD_SLOT));
            }
        }
    }

    public void PressedBoosterMagnetBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
            {
                if (UserConfig.Instance.AmountBoosterMagnet > 0)
                {
                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
                    {
                        tutorialUI.Hide();
                    }

                    //if (ShooterManager.Instance.IsQueueEmpty()) return;

                    //if (!ParkingManager.Instance.IsParkingSlotEmpty())
                    //{
                    //    UIManager.Instance.ShowItemUsingMenu(eBooster.Magnet);
                    //}
                    //else
                    //{
                    //    WarningMenu.Instance.Show(Vector2.zero, "Slots are empty!");
                    //}
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.Magnet);
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET));
            }
        }
    }
    
    public void PressedRemoveAdsBtn()
    {
        //CGTeamBridge.instance.LogEvent("purchase_removeads");
        //CGTeamBridge.instance.SetResumeAds(true);
        //IAPManager.Instance.Purchase(IAPManager.kRemoveAds, () =>
        //{
        //    CGTeamBridge.instance.SetResumeAds(false);
        //});
    }

    public void ShowBottom()
    {
        booster.SetActive(true);
    }

    void SetStateLevel(eTypeLevel eTypeLevel)
    {
        
    }

    void ShowNoticeHardLevel()
    {
        UIManager.Instance.ShowNoticeHardMenu();
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    #region Video
    [Header("Video")]
    [SerializeField] GameObject content;
    public void PressedActiveUI()
    {
        bool active = content.activeSelf;
        content.SetActive(!active);

        if (active)
        {
            EconomyMenu.instance.Hide();
        }
        else
        {
            EconomyMenu.instance.Show();
        }
    }

    public void ActiveUI(bool active)
    {
        content.SetActive(active);
    }
    #endregion

    #region Cheat
    public void PressedCheatBtn()
    {
        float diffTime = Time.time - GameManager.Instance.startTimeCheat;
        if (diffTime > 1)
        {
            GameManager.Instance.tapCheat = 1;
            GameManager.Instance.startTimeCheat = Time.time;
        }
        else
        {
            GameManager.Instance.tapCheat++;
            if (GameManager.Instance.tapCheat == 5)
            {
                // Show Ui Cheat
                UIManager.Instance.ShowCheatMenu();
            }
        }

        GameManager.Instance.startTimeCheat = Time.time;
    }
    #endregion
}
