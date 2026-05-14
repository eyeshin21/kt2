using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    public Vector2Int clothSize;
    public GameObject objModel;
    public GameObject objChain;
    public GameObject objSign;
    public TextMeshPro txtAmount;
    public Transform leftCurtain;
    public Transform rightCurtain;
    public ParticleSystem vfxBreak;

    public Vector2Int pos;
    public int amountCloth;
    public List<Box> boxes = new List<Box>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(EventVariables.CheckCloth, CheckCloth);
    }

    public void Init(ClothData data)
    {
        pos = new Vector2Int(data.coordinateX, data.coordinateY);
        amountCloth = data.clothCount;
        txtAmount.text = amountCloth.ToString();
    }

    public void CheckBox()
    {
        for (int x = pos.x; x < pos.x + clothSize.x; x++)
        {
            for (int y = pos.y; y < pos.y + clothSize.y; y++)
            {
                Box box = BoxManager.Instance.boxGrid[x, y];
                if (box != null)
                {
                    box.hasCloth = true;
                    boxes.Add(box);
                }
            }
        }
    }

    public void CheckCloth()
    {
        if (amountCloth > 0)
        {
            amountCloth--;
            txtAmount.text = amountCloth.ToString();

            objSign.transform.DOKill(true);
            objSign.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f).SetEase(Ease.Linear);
            vfxBreak.Play();

            if (amountCloth == 0)
            {
                Reveal();
            }
        }
    }

    public void Reveal()
    {
        objSign.SetActive(false);
        objChain.SetActive(false);

        leftCurtain.transform.DOScaleX(0f, 0.3f).SetEase(Ease.Linear);
        rightCurtain.transform.DOScaleX(0f, 0.3f).SetEase(Ease.Linear);
        objModel.transform.DOLocalMoveZ(-0.5f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].hasCloth = false;

                if (boxes[i].isActive)
                {
                    boxes[i].Active2();
                }
            }

            objModel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Recycle();
            });
        });

    }

    public void Recycle()
    {
        transform.DOKill();
        leftCurtain.transform.DOKill();
        rightCurtain.transform.DOKill();
        objModel.transform.DOKill();

        leftCurtain.localScale = Vector3.one;
        rightCurtain.localScale = Vector3.one;
        objModel.transform.localScale = Vector3.one;

        objSign.SetActive(true);
        objChain.SetActive(true);

        boxes.Clear();

        amountCloth = 0;

        gameObject.Recycle();
    }
}
