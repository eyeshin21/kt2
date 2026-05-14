using DG.Tweening;
using Lean.Touch;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsingMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] GameObject closeBtn;

    [HideInInspector] public eBooster eBooster;
    [HideInInspector] public bool isTut = false;

    public override void Show()
    {
        base.Show();

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        });

        GameManager.Instance.canControl = false;

        UIManager.Instance.ingameMenu.ActiveUI(false);

        //switch (eBooster)
        //{
        //    case eBooster.HandSelect:
        //        contentTxt.text = "Pick any hooks from the queue.";
        //        ShooterManager.Instance.ActiveForBoosterHandSelect();

        //        GameplayController.Instance.MoveCamera(true, () =>
        //        {
        //            if (isTut)
        //            {
        //                UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("", Vector2.zero, Vector2.zero, Vector2.zero, GameplayController.Instance.canvas.WorldToCanvasPosition(ShooterManager.Instance.boxQueues[1].shooters[2].txtAmount.transform.position, GameplayController.Instance.cam), 0);
        //            }
        //        });
        //        break;
        //    case eBooster.Magnet:
        //        contentTxt.text = "Pick any hooks from the slot.";
        //        ShooterManager.Instance.ActiveForBoosterMagnet();
        //        GameplayController.Instance.MoveCamera(true, () =>
        //        {
        //            if (isTut)
        //            {
        //                UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("", Vector2.zero, Vector2.zero, Vector2.zero, GameplayController.Instance.canvas.WorldToCanvasPosition(ParkingManager.Instance.parkedShooters[0].txtAmount.transform.position, GameplayController.Instance.cam), 0);
        //            }
        //        });
        //        break;
        //}

        closeBtn.SetActive(!isTut);
    }

    void HandleFingerTap(LeanFinger finger)
    {
        Ray ray = finger.GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            //if (hit.collider.transform.parent.TryGetComponent(out ShooterController shooter))
            //{
            //    switch (eBooster)
            //    {
            //        case eBooster.HandSelect:
            //            {
            //                if (!shooter.isActive || shooter.isHidden || shooter.hasIce || shooter.hasCrate)
            //                {
            //                    shooter.Shake();
            //                    return;
            //                }

            //                if (ParkingManager.Instance.TryGetFreeSlot(out ParkingSlot parkingSlot))
            //                {
            //                    ShooterManager.Instance.InActiveForBoosterHandSelect();
            //                    shooter.Active();
            //                    shooter.MoveToParkingSlot(parkingSlot);
            //                }
            //                else
            //                {
            //                    shooter.Shake();
            //                    HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            //                    return;
            //                }

            //                UserConfig.Instance.AmountBoosterHandSelect--;
            //                EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);

            //                //SoundManager.instance.PlaySound("Select");
            //                HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

            //                if (isTut)
            //                {
            //                    isTut = false;
            //                    UIManager.Instance.ingameMenu.tutorialUI.Hide();
            //                }

            //                CGTeamBridge.Instance.TrackUseBooster("hand_select", UserConfig.Instance.CurLevel);

            //                Hide();
            //                break;
            //            }
            //        case eBooster.Magnet:
            //            {
            //                if (ParkingManager.Instance.parkedShooters.Contains(shooter) && shooter.realAmount > 0 && !shooter.isShooting && !shooter.isMoving && !shooter.isUsingMagnet)
            //                {
            //                    shooter.isUsingMagnet = true;
            //                    shooter.CheckShoot();

            //                    UserConfig.Instance.AmountBoosterMagnet--;
            //                    EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);

            //                    //SoundManager.instance.PlaySound("Select");
            //                    HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

            //                    if (isTut)
            //                    {
            //                        isTut = false;
            //                        UIManager.Instance.ingameMenu.tutorialUI.Hide();
            //                    }

            //                    CGTeamBridge.Instance.TrackUseBooster("magnet", UserConfig.Instance.CurLevel);

            //                    Hide();
            //                }
            //                else
            //                {
            //                    shooter.Shake();
            //                    HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            //                }
            //                break;
            //            }
            //    }
            //}
        }
    }

    public override void Hide()
    {
        base.Hide();
        popup.DOKill();
        gameObject.SetActive(false);

        LeanTouch.OnFingerTap -= HandleFingerTap;

        //switch (eBooster)
        //{
        //    case eBooster.HandSelect:
        //        GameplayController.Instance.MoveCamera(false);
        //        //GameplayController.Instance.SetBackground(false);
        //        ShooterManager.Instance.InActiveForBoosterHandSelect();
        //        break;
        //    case eBooster.Magnet:
        //        GameplayController.Instance.MoveCamera(false);
        //        ShooterManager.Instance.InActiveForBoosterMagnet();
        //        break;
        //}

        UIManager.Instance.ingameMenu.ActiveUI(true);
        GameManager.Instance.canControl = true;
    }
}
