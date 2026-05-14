using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;

public class WinMenu : UIMenu
{
    [SerializeField] Image bg;
    [SerializeField] GameObject objEmoji;
    [SerializeField] TextMeshProUGUI[] levelTxts;
    [SerializeField] TextMeshProUGUI descriptionTxt;
    [SerializeField] TextMeshProUGUI coinTxt;

    [Header("New Element")]
    [SerializeField] Transform elementParent;
    [SerializeField] GameObject elementObj;
    [SerializeField] TextMeshProUGUI progressElementTxt;
    [SerializeField] TextMeshProUGUI elementNameTxt;
    [SerializeField] Image elementImg;
    [SerializeField] Image progressElementImg;

    [Header("Spin")]
    [SerializeField] RectTransform arrow;
    [SerializeField] RectTransform posLeft;
    [SerializeField] RectTransform posRight;
    [SerializeField] RectTransform[] rtsfBonus;
    [SerializeField] TextMeshProUGUI[] txtsBonus;
    [SerializeField] TextMeshProUGUI txtCoinClaimAds;

    [Header("Animation")]
    [SerializeField] Transform levelTitle;
    [SerializeField] Transform spin;
    [SerializeField] Transform nextBtn;
    [SerializeField] Transform nextBtnAds;

    bool canContinue;

    int coinBonus = 50;
    int coinSpin = 100;

    ProgressElement progressElement = null;
    float progress = 0;
    float newProgress = 0;

    public override void Show()
    {
        base.Show();
        canContinue = true;

        int timePlay = (int)(Time.time - GameManager.Instance.startTime);
        //Bridge.Instance.OnGameFinished(true, timePlay, UserConfig.Instance.CurLevel);

        int minute = timePlay / 60;
        int second = timePlay % 60;

        string minuteStr = "";
        if (minute > 1)
        {
            minuteStr = minute + " minutes";
        }
        else
        {
            minuteStr = minute + " minute";
        }

        string secondStr = "";
        if (second > 1)
        {
            secondStr = second + " seconds";
        }
        else
        {
            secondStr = second + " second";
        }

        descriptionTxt.text = "It took " + minuteStr + " and " + secondStr;

        elementObj.SetActive(false);


        progressElement = GameManager.Instance.GetProgressElement();
        if (progressElement == null)
        {
            elementObj.SetActive(false);
            objEmoji.SetActive(true);
        }
        else
        {
            elementObj.SetActive(true);
            objEmoji.SetActive(false);

            //progressElementTxt.text = string.Format("{0}/{1}", UserConfig.Instance.CurLevel - progressElement.prevLevel, progressElement.nextLevel - progressElement.prevLevel);
            progress = ((float)(UserConfig.Instance.CurLevel - progressElement.prevLevel)) / (progressElement.nextLevel - progressElement.prevLevel);
            progressElementTxt.text = $"{Mathf.RoundToInt(progress * 100f)}%";

            Sprite sprElement = Resources.Load<Sprite>($"Icons/Elements/{progressElement.eTypeElement.ToString()}");
            elementImg.sprite = sprElement;
            progressElementImg.sprite = sprElement;
            elementImg.SetNativeSize();
            progressElementImg.SetNativeSize();

            switch (progressElement.eTypeElement)
            {
                case eTypeElement.HiddenBox:
                    elementNameTxt.text = "Hidden Hooks";
                    break;
                case eTypeElement.LargeBox:
                    elementNameTxt.text = "Large Hooks";
                    break;
                case eTypeElement.StarBox:
                    elementNameTxt.text = "Big Balls";
                    break;
                case eTypeElement.LinkedBox:
                    elementNameTxt.text = "Linked Hooks";
                    break;
                case eTypeElement.IceBox:
                    elementNameTxt.text = "Ice Baskets";
                    break;
                case eTypeElement.HiddenBlock:
                    elementNameTxt.text = "Hidden Fishes";
                    break;
                case eTypeElement.LockAndKey:
                    elementNameTxt.text = "Lock & Key";
                    break;
                case eTypeElement.HalfBasket:
                    elementNameTxt.text = "Half Baskets";
                    break;
                case eTypeElement.DropBox:
                    elementNameTxt.text = "Drop Box";
                    break;
                case eTypeElement.BasketCrate:
                    elementNameTxt.text = "Baskets Crate";
                    break;
                case eTypeElement.Cloth:
                    elementNameTxt.text = "Cloth";
                    break;
            }

            //if (progress == 0 || elementParent.childCount == 0)
            //{
            //    if (elementParent.childCount > 0)
            //    {
            //        elementParent.GetChild(0).gameObject.Recycle();
            //    }
            //    GameObject tempElementObj = GameManager.Instance.InstantiatePrefab("Icons/Elements/" + progressElement.eTypeElement);
            //    tempElementObj.transform.parent = elementParent;
            //    tempElementObj.transform.localPosition = Vector3.zero;
            //    tempElementObj.transform.localScale = Vector3.one;
            //}

            progressElementImg.fillAmount = progress;
            int tempNewProgress = UserConfig.Instance.CurLevel + 1 - progressElement.prevLevel;
            float newProgress = (float)tempNewProgress / (progressElement.nextLevel - progressElement.prevLevel);

            progressElementImg.DOKill();
            progressElementImg.DOFillAmount(newProgress, 0.5f).SetEase(Ease.Linear);
            DOVirtual.Float(progress, newProgress, 0.5f, result =>
            {
                progressElementTxt.text = $"{Mathf.RoundToInt(result * 100f)}%";
            }).SetEase(Ease.Linear);
        }


        coinBonus = GameManager.Instance.GetCoinRewardLevel(UserConfig.Instance.CurLevel);
        coinSpin = coinBonus * 2;

        for (int i = 0; i < levelTxts.Length; i++)
        {
            levelTxts[i].text = "Level " + UserConfig.Instance.CurLevel;
        }
        UserConfig.Instance.WinLevel++;
        UserConfig.Instance.CurLevel++;
        UserConfig.Instance.AmountWinStreak++;

        coinTxt.text = $"+{coinBonus}";
        txtCoinClaimAds.text = $"+{coinSpin}";

        nextBtnAds.gameObject.SetActive(UserConfig.Instance.CurLevel > 15);

        // Anim UI
        bg.color = new Color(0, 0f, 0f, 0);
        bg.DOFade(0.95f, 0.25f).SetEase(Ease.Linear);
        ShowAnimWinGame();

        if (UserConfig.Instance.CurLevel >= GameManager.MAX_LEVEL)
        {
            UserConfig.Instance.NextLevel = -1;
        }
    }

    void ShowAnimWinGame()
    {
        levelTitle.localScale = Vector3.zero;
        spin.localScale = Vector3.zero;
        nextBtn.localScale = Vector3.zero;
        nextBtnAds.localScale = Vector3.zero;

        //arrow.DOKill();
        //arrow.anchoredPosition = new Vector2(0, arrow.anchoredPosition.y);
        //spin.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.25f).OnComplete(() =>
        //{
        //    Spin();
        //});

        levelTitle.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        nextBtnAds.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
        nextBtn.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.75f);
    }

    private void Spin()
    {
        for (int i = 0; i < txtsBonus.Length; i++)
        {
            if (i == 2)
            {
                txtsBonus[i].fontSize = 50;
            }
            else
            {
                txtsBonus[i].fontSize = 35;
            }
        }

        coinSpin = Mathf.RoundToInt(coinBonus * 3f);
        txtCoinClaimAds.text = string.Format("{0}", coinSpin);

        arrow.DOAnchorPosX(posRight.anchoredPosition.x, 0.5f).SetEase(Ease.InQuad).SetUpdate(true)
            .OnUpdate(() => ArrowCallBack())
            .OnComplete(() =>
            {
                arrow.DOAnchorPosX(posLeft.anchoredPosition.x, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetUpdate(true).OnUpdate(() => ArrowCallBack());
            });
    }

    private void ArrowCallBack()
    {
        for (int i = 0; i < txtsBonus.Length; i++)
        {
            txtsBonus[i].fontSize = 35;
        }

        if (arrow.anchoredPosition.x <= rtsfBonus[0].anchoredPosition.x)
        {
            coinSpin = Mathf.RoundToInt(coinBonus * 1.5f);
            txtsBonus[0].fontSize = 50;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[0].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[1].anchoredPosition.x)
        {
            txtsBonus[1].fontSize = 50;
            coinSpin = coinBonus * 2;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[1].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[2].anchoredPosition.x)
        {
            txtsBonus[2].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 2.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[2].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[4].anchoredPosition.x)
        {
            txtsBonus[3].fontSize = 50;
            coinSpin = coinBonus * 3;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[4].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[5].anchoredPosition.x)
        {
            txtsBonus[4].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 2.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[5].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[6].anchoredPosition.x)
        {
            txtsBonus[5].fontSize = 50;
            coinSpin = coinBonus * 2;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[6].anchoredPosition.x)
        {
            txtsBonus[6].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 1.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
    }

    public void PressedNextBtn()
    {
        if (canContinue)
        {
            canContinue = false;
            arrow.DOKill();

            UnityEvent onDoneAddGold = new UnityEvent();
            onDoneAddGold.AddListener(() =>
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(() =>
                {
                    Hide();
                    GameManager.Instance.LoadLevel();
                    UIManager.Instance.ShowIngameMenu();
                });
                FadeMenu.Instance.Fade(e, true);
            });
            EconomyMenu.instance.AddGold(coinBonus, coinTxt.transform, onDoneAddGold);

            CGTeamBridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel - 1, "currency", "coin", coinBonus, "reward", "win_menu", UserConfig.Instance.Coin);
        }
    }

    public void PressedCoinAdsBtn()
    {
        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            SoundManager.instance.PlaySound("GetReward");

            if (canContinue)
            {
                canContinue = false;

                arrow.DOKill();

                coinTxt.text = $"+{coinSpin}";
                coinTxt.transform.DOKill(true);
                coinTxt.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 1).SetEase(Ease.Linear);

                UnityEvent onDoneAddGold = new UnityEvent();
                onDoneAddGold.AddListener(() =>
                {
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        Hide();
                        GameManager.Instance.LoadLevel();
                        UIManager.Instance.ShowIngameMenu();
                    });
                    FadeMenu.Instance.Fade(e, true);
                });
                EconomyMenu.instance.AddGold(coinSpin, coinTxt.transform, onDoneAddGold);

                CGTeamBridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel - 1, "currency", "coin", coinSpin, "watch_ads", "win_menu", UserConfig.Instance.Coin);
            }
        });
        CGTeamBridge.Instance.ShowRewarded("x2_reward", null, e, null);

        //if (UserConfig.Instance.Ticket > 0)
        //{
        //    EconomyMenu.instance.AddTicket(-1);
        //    EconomyMenu.instance.TicketFly(coinAdsTF.anchoredPosition, e);
        //}
        //else
        //{
        //    CGTeamBridge.instance.ShowRewarded("reward_coin", null, e, null);
        //}
    }

    public void PressedRaceBtn()
    {
        //tutorialRect.gameObject.SetActive(false);
        //if (UserConfig.Instance.IsRacing)
        //{
        //    UIManager.Instance.ShowRaceMenu();
        //}
        //else
        //{
        //    UIManager.Instance.ShowStartRaceMenu();
        //}
    }

    public void PressedHomeBtn()
    {
        //tutorialRect.gameObject.SetActive(false);
        //UnityEvent e = new UnityEvent();
        //e.AddListener(() =>
        //{
        //    Hide();
        //    GameManager.Instance.RecycleLevel();
        //    UIManager.Instance.ShowMainMenu();
        //});
        //FadeMenu.Instance.Fade(e);
    }

    public override void Hide()
    {
        base.Hide();
        arrow.DOKill();
        gameObject.SetActive(false);
    }
}
