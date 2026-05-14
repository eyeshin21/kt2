using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum eTypeBooster
{
    AddSlot = 0,
    HandSelect = 1,
    DoubleCart = 2,
    Magnet = 3
}

public class NewBoosterMenu : UIMenu
{
    [HideInInspector] public eTypeBooster eTypeBooster;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CanvasGroup canvasGroup2;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] RectTransform rtsfIcon;
    [SerializeField] Image iconImg;
    [SerializeField] AnimatedButton btnClaim;

    [Header("VFX")]
    [SerializeField] RectTransform rtsfVfxAddAmount;
    [SerializeField] ParticleSystem vfxAddAmount;

    [Header("Tutorial")]
    [SerializeField] RectTransform handRect;
    [SerializeField] Animator animHand;
    [SerializeField] GameObject labelTut;
    [SerializeField] TextMeshProUGUI txtTut;

    [Header("Booster")]
    [SerializeField] GameObject boosterAddSlot;
    [SerializeField] GameObject boosterHandSelect;
    [SerializeField] GameObject boosterMagnet;

    [SerializeField] TextMeshProUGUI txtAmountBoosterAddSlot;
    [SerializeField] TextMeshProUGUI txtAmountBoosterHandSelect;
    [SerializeField] TextMeshProUGUI txtAmountBoosterMagnet;

    public override void Show()
    {
        base.Show();

        boosterAddSlot.SetActive(false);
        boosterHandSelect.SetActive(false);
        boosterMagnet.SetActive(false);

        animHand.gameObject.SetActive(false);
        labelTut.SetActive(false);

        switch (eTypeBooster)
        {
            case eTypeBooster.AddSlot:
                nameTxt.text = "Add Slot";
                contentTxt.text = "Gain a temporary slot for one hook.";
                txtTut.text = "";
                break;
            case eTypeBooster.HandSelect:
                nameTxt.text = "Hand Select";
                contentTxt.text = "Pick any hooks from the queue.";
                txtTut.text = "";
                break;
            case eTypeBooster.Magnet:
                nameTxt.text = "Magnet";
                contentTxt.text = "Pick any hooks from the slot to shoot.";
                txtTut.text = "";
                break;
        }

        iconImg.sprite = GameManager.Instance.LoadSprite("Icons/Boosters/" + eTypeBooster);
        iconImg.SetNativeSize();

        canvasGroup2.alpha = 1;
        rtsfIcon.gameObject.SetActive(true);
        rtsfIcon.anchoredPosition = new Vector2(0, 128.65f);
        rtsfIcon.sizeDelta = new Vector2(250, 250);

        btnClaim.interactable = true;

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.Linear);
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    public void OnClickButtonClaim()
    {
        btnClaim.interactable = false;

        Vector2 target = Vector2.zero;
        switch (eTypeBooster)
        {
            case eTypeBooster.HandSelect:
                target = new Vector2(-224f, 138f - GameManager.Instance.screenHeight / 2.0f);// + GameManager.Instance.safeAreaYBottom);
                break;
            case eTypeBooster.Magnet:
                target = new Vector2(0f, 138f - GameManager.Instance.screenHeight / 2.0f);// + GameManager.Instance.safeAreaYBottom);
                break;
            case eTypeBooster.AddSlot:
                target = new Vector2(224f, 138f - GameManager.Instance.screenHeight / 2.0f);// + GameManager.Instance.safeAreaYBottom);
                break;
        }

        canvasGroup2.DOFade(0f, 0.5f).SetEase(Ease.Linear);
        rtsfIcon.DOSizeDelta(new Vector2(110, 110), 0.5f).SetEase(Ease.Linear);
        rtsfIcon.DOAnchorPos(target, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            //if (eTypeBooster == eTypeBooster.Clear)
            //{
            //    Hide();
            //    return;
            //}

            //labelTut.SetActive(true);

            rtsfIcon.gameObject.SetActive(false);
            handRect.gameObject.SetActive(true);
            handRect.anchoredPosition = target;
            animHand.Play("Base Layer.TutorialTap", 0, 0);

            rtsfVfxAddAmount.anchoredPosition = target;
            vfxAddAmount.Play();

            switch (eTypeBooster)
            {
                case eTypeBooster.AddSlot:
                    boosterAddSlot.SetActive(true);
                    txtAmountBoosterAddSlot.text = UserConfig.Instance.AmountBoosterAddSlot.ToString();
                    break;
                case eTypeBooster.HandSelect:
                    boosterHandSelect.SetActive(true);
                    txtAmountBoosterHandSelect.text = UserConfig.Instance.AmountBoosterHandSelect.ToString();
                    break;
                case eTypeBooster.Magnet:
                    boosterMagnet.SetActive(true);
                    txtAmountBoosterMagnet.text = UserConfig.Instance.AmountBoosterMagnet.ToString();
                    break;
            }
        });
    }

    public void OnClickButtonBoosterAddSlot()
    {
        Hide();
        UIManager.Instance.ingameMenu.PressedBoosterAddSlotBtn();
        CGTeamBridge.Instance.TrackTutAction("use_booster_add_slot");
    }

    public void OnClickButtonBoosterHandSelect()
    {
        Hide();
        UIManager.Instance.ShowItemUsingMenu(eBooster.HandSelect, true);
        CGTeamBridge.Instance.TrackTutAction("use_booster_hand_select");
    }

    public void OnClickButtonBoosterMagnet()
    {
        Hide();
        UIManager.Instance.ShowItemUsingMenu(eBooster.Magnet, true);
        CGTeamBridge.Instance.TrackTutAction("use_booster_magnet");
    }
}
