using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum eTypeElement
{
    None = 0,
    HiddenBox = 1,
    LargeBox = 2,
    StarBox = 3,
    LinkedBox = 4,
    IceBox = 5,
    HiddenBlock = 6,
    LockAndKey = 7,
    HalfBasket = 8,
    DropBox = 9,
    BasketCrate = 10,
    Cloth = 11
}

public class NewElementMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] Transform parentNewElement;

    [HideInInspector] public eTypeElement eTypeElement;

    public override void Show()
    {
        base.Show();

        switch (eTypeElement)
        {
            case eTypeElement.HiddenBox:
                nameTxt.text = "Hidden Hooks";
                contentTxt.text = "Hidden Hooks hold their mystery until they get to the front row.";
                break;
            case eTypeElement.LargeBox:
                nameTxt.text = "Large Hooks";
                contentTxt.text = "Large Hooks require twice as many balls to get filled.";
                break;
            case eTypeElement.StarBox:
                nameTxt.text = "Big Balls";
                contentTxt.text = "";
                break;
            case eTypeElement.LinkedBox:
                nameTxt.text = "Linked Hooks";
                contentTxt.text = "Linked Hooks will move together when one of them is tapped.";
                break;
            case eTypeElement.IceBox:
                nameTxt.text = "Ice Hooks";
                contentTxt.text = "Break the ice by moving a certain number of hooks.";
                break;
            case eTypeElement.HiddenBlock:
                nameTxt.text = "Hidden Blocks";
                contentTxt.text = "Hidden Blocks reveal themselves at the bottom row.";
                break;
            case eTypeElement.LockAndKey:
                nameTxt.text = "Lock & Key";
                contentTxt.text = "Locks block the path until its Key drops to the bottom.";
                break;
            case eTypeElement.HalfBasket:
                nameTxt.text = "Half Baskets";
                contentTxt.text = "Half Baskets must merge in a slot before they can collect fishes.";
                break;
            case eTypeElement.DropBox:
                nameTxt.text = "Drop Box";
                contentTxt.text = "Contains items inside and drops them onto nearby tiles when triggered.";
                break;
            case eTypeElement.BasketCrate:
                nameTxt.text = "Baskets Crate";
                contentTxt.text = "Break the crate by moving a certain number of baskets.";
                break;
            case eTypeElement.Cloth:
                nameTxt.text = "Cloth";
                contentTxt.text = "Cloth hides the upcoming rows of fishes.";
                break;
        }

        if (parentNewElement.childCount > 0)
        {
            parentNewElement.GetChild(0).gameObject.Recycle();
        }

        GameObject obj = GameManager.Instance.InstantiatePrefab("Icons/Elements/" + eTypeElement);
        obj.transform.parent = parentNewElement;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
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
